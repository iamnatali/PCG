using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PCG
{

    public class DiamondSquareGenerator:HeightMapGenerator
    {
        private  int size;
        private  double[][] map;
        public  int roughness { get; set; }
        private  static Random random = new Random();

        public DiamondSquareGenerator() { }

        public DiamondSquareGenerator(int roughn)
        {
            roughness = roughn;
            if (roughness <= 0)
                throw new ArgumentException("wrong parameter");
        }

        private void InitHeightMap(int size)
        {
            map = new double[size][];
            for (int i = 0; i < size; i++)
                map[i] = new double[size];
        }

        private void SquareStep(int lx, int ly, int rx, int ry)
        {
            int l = (rx - lx) / 2;
            var p1 = map[lx][ly];              
            var p2 = map[lx][ry];              
            var p3 = map[rx][ry];              
            var p4 = map[rx][ly];
            map[lx + l][ly + l] = (p1 + p2 + p3 + p4) / 4 
                + random.Next(-l * roughness, l * roughness);
        }

        private void DiamondStep(int x, int y, int l)
        {
            var p1 = y - l >= 0 ? map[x][y - l] : map[x][size - l];
            var p2 = x - l >= 0 ? map[x - l][y] : map[size - l][y];
            var p3 = map[x][(y + l) % size];
            var p4 = map[(x + l) % size][y];
            map[x][ y] = (p1 + p2 + p3 + p4) / 4 
                + random.Next(-l * roughness, l * roughness);
        }

        private void DiamondSquare(int lx, int ly, int rx, int ry)
        {
            int l = (rx - lx) / 2;
            if (l > 0)
            {
                SquareStep(lx, ly, rx, ry);
                DiamondStep(lx, ly + l, l);
                DiamondStep(rx, ry - l, l);
                DiamondStep(rx - l, ry, l);
                DiamondStep(lx + l, ly, l);
            }
        }

        private void Fill(int s=1025)
        {
            size = s;
            InitHeightMap(size);
            map[0][0] = random.Next(-roughness, roughness);
            map[size - 1][ 0] = random.Next(-roughness, roughness);
            map[0][ size - 1] = random.Next(-roughness, roughness);
            map[size - 1][size - 1] = random.Next(-roughness, roughness);
            for (int l = (size - 1) / 2; l > 0; l /= 2)
                for (int x = 0; x < size - 1; x += l)
                    for (int y = 0; y < size - 1; y += l)
                        DiamondSquare(x, y, x + l, y + l);

        }



        public double[][] GetMap(int width = 1025, int height=1025)
        {
            if (width != height)
                throw new ArgumentException("Width should be equal to height");
            var powerCheck = width - 1;
            if ((powerCheck & (powerCheck - 1)) != 0)
                throw new ArgumentException("Width and height should be (power of two)+1");
            Fill(width);
            return map;
        }
    }
}
