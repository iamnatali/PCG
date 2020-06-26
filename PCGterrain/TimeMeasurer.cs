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
        public void RunNTimes(Func<double[][]> action, int n)
        {
            sv = new StatValue();
            action();
            for (int i=0; i<n; i++)
            {
                RunAndTime(action);
            }
        }

        public double RunAndTime(Func<double[][]> action)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var m = action();
            stopWatch.Stop();
            var elapsed = stopWatch.ElapsedMilliseconds;
            sv.Add(elapsed);
            return elapsed;
        }
    }
}
