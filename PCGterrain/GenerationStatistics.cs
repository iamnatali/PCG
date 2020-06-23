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
        List<HeightMapGenerator> testList = new List<HeightMapGenerator>()
        {
            new BilinearValueInterpolationGenerator(33, 200, 0),
            new NonlinearValueInterpolationGenerator(33, 200, 0),
            new CosineValueInterpolationGenerator(33,200,0),
            new HillGenerator(20,200, 1800),
            new PerlinGradientInterpolationGenerator(33, 200, 0),
            new DiamondSquareGenerator(200)
        };

        public string GetStatistics(HeightMapGenerator inter, int n, int width, int height)
        {
            var tm = new TimeMeasurer();
            tm.RunNTimes(() => inter.GetMap(width, height), n);
            Console.WriteLine(inter.GetType().Name);
            Console.WriteLine(tm.sv.ToDetailedString());
            return tm.sv.ToDetailedString();
        }
    }
}
