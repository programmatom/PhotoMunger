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
using System.Text;

namespace AdaptiveImageSizeReducer
{
    // Hierarchical elapsed time measurement

    [Serializable]
    public class Profile
    {
        private Entry top;
        private Entry current;
        private Stack<Entry> parents = new Stack<Entry>();

        [Serializable]
        private class MyStopWatch
        {
            private long elapsedMsec;
            [NonSerialized]
            private readonly Stopwatch sw;

            public MyStopWatch(Stopwatch sw)
            {
                this.sw = sw;
                if (!sw.IsRunning)
                {
                    this.elapsedMsec = sw.ElapsedMilliseconds;
                }
            }

            public MyStopWatch(MyStopWatch copyFrom)
            {
                this.elapsedMsec = copyFrom.elapsedMsec;
                this.sw = copyFrom.sw;
            }

            public static MyStopWatch StartNew()
            {
                return new MyStopWatch(Stopwatch.StartNew());
            }

            public void Stop()
            {
                sw.Stop();
                elapsedMsec = sw.ElapsedMilliseconds;
            }

            public long ElapsedMilliseconds { get { return elapsedMsec; } }
        }

        [Serializable]
        private class Entry
        {
            public readonly string label;
            public readonly MyStopWatch cumulative;
            public readonly List<Entry> subs;

            public Entry(string label)
            {
                this.label = label;
                this.cumulative = MyStopWatch.StartNew();
                this.subs = new List<Entry>();
            }

            public Entry(string label, MyStopWatch time)
            {
                this.label = label;
                this.cumulative = time;
                this.subs = new List<Entry>();
            }

            public void End()
            {
                cumulative.Stop();
            }
        }

        public Profile(string label)
        {
            Push(label);
        }

        public Profile(string format, params object[] args)
            : this(String.Format(format, args))
        {
        }

        public Profile()
            : this(String.Empty)
        {
        }

        public void Push(string label)
        {
            Debug.Assert((top == null) == (current == null));

            Entry level = new Entry(label);
            if (current != null)
            {
                current.subs.Add(level);
            }
            parents.Push(current);
            current = level;

            if (top == null)
            {
                top = current;
            }
        }

        public void Push(string format, params object[] args)
        {
            Push(String.Format(format, args));
        }

        public void Pop()
        {
            Debug.Assert(current != null);
            current.End();
            current = parents.Pop();
        }

        public void Add(Stopwatch time, string label)
        {
            current.subs.Add(new Entry(label, new MyStopWatch(time)));
        }

        public void Add(Stopwatch time, string format, params object[] args)
        {
            Add(time, String.Format(format, args));
        }

        public void Add(Profile copyFrom)
        {
            copyFrom.End();

            Stack<List<Entry>> stack = new Stack<List<Entry>>();
            stack.Push(new List<Entry>(new Entry[] { copyFrom.top }));
            parents.Push(current);
            while (stack.Count != 0)
            {
                List<Entry> list = stack.Pop();
                if (list.Count == 0)
                {
                    current = parents.Pop();
                    continue;
                }
                Entry item = list[0];
                list.RemoveAt(0);
                stack.Push(list);

                Entry level = new Entry(item.label, item.cumulative);
                current.subs.Add(level);
                parents.Push(current);
                current = level;

                stack.Push(new List<Entry>(item.subs));
            }
        }

        public void End()
        {
            while (current != null)
            {
                Pop();
            }
        }

        public string Report()
        {
            StringBuilder sb = new StringBuilder();

            Stack<List<Entry>> stack = new Stack<List<Entry>>();
            stack.Push(new List<Entry>(new Entry[] { top }));
            while (stack.Count != 0)
            {
                List<Entry> list = stack.Pop();
                if (list.Count == 0)
                {
                    continue;
                }
                Entry item = list[0];
                list.RemoveAt(0);
                stack.Push(list);

                sb.AppendLine(String.Format("{0}{1}: {2:N3}", new String(' ', 4 * (stack.Count - 1)), item.label, item.cumulative.ElapsedMilliseconds * .001));

                stack.Push(new List<Entry>(item.subs));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return Report();
        }
    }
}
