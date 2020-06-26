using PCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCGterrain
{
    class MapAdder
    {
        public double[][] Map { get; private set;}
        public GenerationStatistics generationStatistics;
        public int sizeX { get; private set; }
        public int sizeY { get; private set; }

        public MapAdder(int sx, int sy)
        {
            sizeX = sx;
            sizeY = sy;
            Map = new double[sizeX][];
            for (int x = 0; x < sizeX; x++)
                Map[x] = new double[sizeY];
            generationStatistics = new GenerationStatistics();
        }

        public void Add(double[][] toAdd, double coeff, 
            HeightMapGenerator gen, int n)
        {
            if (toAdd.Length != sizeX || toAdd[0].Length != sizeY)
                throw new ArgumentException("wrong size of map is trying to be added");
            for (int x=0;x<sizeX;x++)
                for (int y = 0; y < sizeY; y++)
                {
                    Map[x][y]+=coeff*toAdd[x][y];
                }
            generationStatistics.GetStatistics(gen, n, toAdd.Length, toAdd[0].Length);
        }
    }
}
