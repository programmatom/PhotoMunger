/*
 *  Copyright © 2010-2016 Thomas R. Lawrence
 * 
 *  GNU General Public License
 * 
 *  This file is part of PhotoMunger
 * 
 *  PhotoMunger is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AdaptiveImageSizeReducer
{
    public class BatchAnalyzerQueue
    {
        private readonly int total;
        private readonly List<Item> queue;
        private readonly CancellationTokenSource cancel = new CancellationTokenSource();
        private Task<bool> task;

        private BatchAnalyzerQueue(IList<Item> items)
        {
            this.total = items.Count;

            this.queue = new List<Item>(items.Count);
            foreach (Item item in items)
            {
                this.queue.Add(item);
            }
            this.queue.Reverse();
        }

        private void Start()
        {
            for (int i = 0; i < queue.Count; i++)
            {
                Item item = queue[i];
                item.ResetAnalyzeTask(true/*invalidateCurrentView*/);
            }

            this.task = new Task<bool>(
                delegate ()
                {
                    int generated = 0;
                    bool cancelled = false;

                    try
                    {
                        Parallel.For(
                            0,
                            queue.Count,
                            Program.GetProcessorConstrainedParallelOptions(cancel.Token),
                            delegate (int i)
                            {
                                Item item;
                                lock (queue)
                                {
                                    int index = queue.Count - 1;
                                    item = queue[index];
                                    queue.RemoveAt(index);
                                }

                                item.WaitInit();

                                Interlocked.Increment(ref generated);
                            });
                    }
                    catch (OperationCanceledException)
                    {
                        cancelled = true;
                    }


                    Debug.Assert(cancelled || cancel.IsCancellationRequested || (generated == this.total));
                    return false;
                },
                cancel.Token);
            this.task.Start();
        }

        public void Prioritize(Item item)
        {
            lock (queue)
            {
                int i = queue.IndexOf(item);
                if (i >= 0)
                {
                    queue.RemoveAt(i);
                    queue.Add(item);
                }
            }
        }

        public bool IsCompleted { get { return this.task.IsCompleted; } }

        public void Wait()
        {
            this.task.Wait();
        }

        public void Cancel()
        {
            this.cancel.Cancel();
        }

        public static BatchAnalyzerQueue BeginAnalyzeBatch(IList<Item> items)
        {
            BatchAnalyzerQueue queue = new BatchAnalyzerQueue(items);
            queue.Start();
            return queue;
        }
    }
}
