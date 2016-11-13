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
using System.Threading;
using System.Threading.Tasks;

namespace AdaptiveImageSizeReducer
{
    // The .NET Parallel.For() method aggressively promotes individual iteration tasks using the "Attached child"
    // concept https://msdn.microsoft.com/en-us/library/dd997417(v=vs.110).aspx which makes those tasks run sooner
    // than globally scheduled tasks. See https://msdn.microsoft.com/en-us/library/system.threading.tasks.taskscheduler(v=vs.110).aspx#Remarks
    // This is usually more performant, but in our case, tasks are fairly long-running and there are also UI
    // tasks that need to be done promptly. The built-in version starves the UI tasks, but this "inefficient" implementation
    // avoids prioritization of child tasks which gives much better UI responsiveness.

    public static class ParallelDevolved
    {
        public static void For(int start, int count, CancellationToken cancel, Action<int> body)
        {
            int p = !Program.ProfileMode ? Environment.ProcessorCount : 1;

            EventWaitHandle[] tasksAvailability = new EventWaitHandle[p];

            for (int i = 0; i < p; i++)
            {
                tasksAvailability[i] = new EventWaitHandle(true, EventResetMode.AutoReset);
            }

            for (int i = 0; i < count; i++)
            {
                if (cancel.IsCancellationRequested)
                {
                    break;
                }

                int t = EventWaitHandle.WaitAny(tasksAvailability);
                tasksAvailability[t].Reset();
                Task<bool> task = new Task<bool>(
                    delegate ()
                    {
                        try
                        {
                            body(i);
                        }
                        finally
                        {
                            tasksAvailability[t].Set();
                        }
                        return false;
                    });
                task.Start();
            }

            EventWaitHandle.WaitAll(tasksAvailability);

            for (int i = 0; i < p; i++)
            {
                tasksAvailability[i].Dispose();
            }
        }
    }
}
