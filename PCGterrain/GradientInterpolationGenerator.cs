using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG
{
    public class PerlinGradientInterpolationGenerator : GradientInterpolationGenerator
    {
        public PerlinGradientInterpolationGenerator() { }

        public PerlinGradientInterpolationGenerator(int d, int u, int l) : base(d, u, l)
        { }

        public override double[][] Interpolate((double, double)[][] toFill)
        {
            //return BasicInterpolate(toFill, x => 3 * x * x - 2 * x * x * x);
            return BasicInterpolate(toFill, x => 6 * Math.Pow(x, 5) - 15 * Math.Pow(x, 4) + 10 * x * x * x);
            //return BasicInterpolate(toFill, x => x);
        }
    }

    public abstract class GradientInterpolationGenerator : InterpolationMapGenerator
    {
        public GradientInterpolationGenerator(int d, int u, int l) : base(d, u, l)
        {
        }
        public GradientInterpolationGenerator() { }

        public abstract double[][] Interpolate((double, double)[][] toFill);

        private static double Fade(int xLength, int yLength, int x, int y,
            double z1, double z2, double z3, double z4)
        {
            Func<double, double> d = t => 3 * t * t - 2 * t * t * t;
            var sizeX = xLength;
            var sizeY = yLength;
            z1 = d(1 - (double)x / sizeX) * d(1 - (double)y / sizeY) * z1;
            z2 = d((double)x / sizeX) * d(1 - (double)y / sizeY) * z2;
            z3 = d(1 - (double)x / sizeX) * d((double)y / sizeY) * z3;
            z4 = d((double)x / sizeX) * d((double)y / sizeY) * z4;
            var z = z1 + z2 + z3 + z4;
            return z;
        }

        public static double[][] BasicInterpolate((double, double)[][] gradient,
            Func<double, double> s)
        {
            var xLength = gradient.Length;
            var yLength = gradient[0].Length;
            var g1 = gradient[0][0];
            var g2 = gradient[xLength - 1][0];
            var g3 = gradient[0][yLength - 1];
            var g4 = gradient[xLength - 1][yLength - 1];
            var xRange = Enumerable
                .Range(0, xLength);
            var yRange = Enumerable
                .Range(0, yLength);
            var arrayToFill = new double[xLength][];
            foreach (var x in xRange)
            {
                arrayToFill[x] = new double[yLength];
                foreach (var y in yRange)
                {
                    var x1 = (x - (xLength - 1));
                    var y1 = (y - (yLength - 1));
                    var z1 = x * g1.Item1 + y * g1.Item2;
                    var z2 = x1 * g2.Item1 + y * g2.Item2;
                    var z3 = x * g3.Item1 +  y1* g3.Item2;
                    var z4 = x1 * g4.Item1 + y1 * g4.Item2;
                    var fz1 = InterpolationStep_GetZ(z1, z2, 0, xLength, x, s);
                    var fz2 = InterpolationStep_GetZ(z3, z4, 0, xLength, x, s);
                    var z = InterpolationStep_GetZ(fz1, fz2, 0, yLength, y, s);
                    arrayToFill[x][y] = z;
                }
            }
            return arrayToFill;
        }

        public override double[][] GetMap(int width, int height)
        {
            if (!DimensionIsOK(width, delta) || !DimensionIsOK(height, delta))
                throw new ArgumentException("wrong dimesion");
            var widthCount = GetLaticeCount(width, delta);
            var heightCount = GetLaticeCount(height, delta);
            var randomArray = MakePairedRandomArray(widthCount, heightCount,
                    lowerBound, upperBound);
            var res = new double[width][];
            int cur = 0;
            for (int x = 1; x < widthCount; x++)
            {
                var yLineDown = MakeEmptyArray(delta);
                for (int y = 1; y < heightCount; y++)
                {
                    var fourPointsArray = MakePairedFourPointsArray(delta, x, y, randomArray);
                    var interpolatedPiece = Interpolate(fourPointsArray);
                    if (yLineDown[0].Length == 0)
                        yLineDown = AddYLine(yLineDown, interpolatedPiece, line => line);
                    else
                        yLineDown = AddYLine(yLineDown, interpolatedPiece, line => line.Skip(1));
                }
                if (x == 1) res[0] = yLineDown[0];
                for (int i = 1; i < delta; i++)
                    res[cur + i] = yLineDown[i];
                cur += delta - 1;
            }
            return res;
        }

        private static (double, double)[,] MakePairedRandomArray(int width, int height,
            int lBound, int uBound)
        {
            var rnd = new Random();
            var randomArray = new (double, double)[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    randomArray[x, y] = (rnd.Next(lBound, uBound), rnd.Next(lBound, uBound));
            return randomArray;
        }

        private static (double, double)[][] MakePairedFourPointsArray(int delta, int x, int y,
            (double, double)[,] randomArray)
        {
            var fourPointsArray = new (double, double)[delta][];
            for (int j = 0; j < delta; j++)
                fourPointsArray[j] = new (double, double)[delta];
            fourPointsArray[0][0] = randomArray[(x - 1), (y - 1)];
            fourPointsArray[0][delta - 1] = randomArray[(x - 1), y];
            fourPointsArray[delta - 1][0] = randomArray[x, y - 1];
            fourPointsArray[delta - 1][delta - 1] = randomArray[x, y];
            return fourPointsArray;
        }
    }
}
