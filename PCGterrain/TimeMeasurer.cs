using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCGterrain
{
    public class TimeMeasurer
    {
        public StatValue sv;
        public TimeMeasurer()
        {
            sv = new StatValue();
        }
        public void RunNTimes(Action action, int n)
        {
            action();
            for (int i=0; i<n; i++)
            {
                RunAndTime(action);
            }
        }

        public double RunAndTime(Action action)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            var elapsed = stopWatch.ElapsedMilliseconds;
            sv.Add(elapsed);
            return elapsed;
        }
    }
}
