using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG
{
    class HillGenerator:HeightMapGenerator
    {
        private int mapSizeX;
        private int mapSizeY;
        private double[][] map;
        public int maxRadius { get; set; }
        public  int minRadius { get; set; }
        public  int stepsCount { get; set; }

        public HillGenerator() { }

        public HillGenerator(int min, int max, int stepsC)
        {
            minRadius = min;
            maxRadius = max;
            stepsCount = stepsC;
        }

        private void InitHeightMap(int width, int height)
        {
            map = new double[width][];
            for (int i = 0; i < width; i++)
                map[i] = new double[height];
        }

        private void RaiseHill(int r, int xC, int yC)
        {
            for (int x=0;x<mapSizeX;x++)
                for (int y=0;y<mapSizeY;y++)
                {
                    var z = r * r - ((x - xC) * (x - xC) + (y - yC)*(y-yC));
                    if (z>0)
                        map[x][y] =map[x][y]+z;
                }
        }

        private void Fill(int width, int height)
        {
            mapSizeX = width;
            mapSizeY = height;
            InitHeightMap(width, height);
            var random = new Random();
            for (int k = 0; k < stepsCount; k++)
            {
                var randomX = random.Next(0, mapSizeX - 1);
                var randomY = random.Next(0, mapSizeY - 1);
                var randomRad = random.Next(minRadius, maxRadius);
                RaiseHill(randomRad, randomX, randomY);
            }
        }

        public double[][] GetMap(int width = 1025, int height=1025)
        {
            if (minRadius > maxRadius)
                throw new ArgumentException("Min radius > Max radius");
            Fill(width, height);
            return map;
        }
    }
}
