using PCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PCGterrain
{
    public class GenerationStatistics
    {
        public TimeMeasurer TimeMeasurer { get; private set; }

        public GenerationStatistics()
        {
            TimeMeasurer = new TimeMeasurer();
        }

        public string GetStatistics(HeightMapGenerator inter, int n, int width, int height)
        {
            TimeMeasurer.RunNTimes(() => {
                var w = inter.GetMap(width, height);
                }, n);
            Console.WriteLine(inter.GetType().Name);
            Console.WriteLine(TimeMeasurer.sv.ToDetailedString());
            return TimeMeasurer.sv.ToDetailedString();
        }
    }
}
