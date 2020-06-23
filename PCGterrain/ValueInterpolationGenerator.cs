using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG
{
    public class BilinearValueInterpolationGenerator : ValueInterpolationGenerator
    {
        public BilinearValueInterpolationGenerator()
        {

        }

        public BilinearValueInterpolationGenerator(int d, int u, int l) : base(d, u, l)
        { }

        public override double[][] Interpolate(double[][] toFill)
        {
            return BasicInterpolate(toFill, x => x);
        }
    }

    public class NonlinearValueInterpolationGenerator : ValueInterpolationGenerator
    {
        public NonlinearValueInterpolationGenerator()
        {}

        public NonlinearValueInterpolationGenerator(int d, int u, int l) : base(d, u, l)
        { }

        public override double[][] Interpolate(double[][] toFill)
        {
            return BasicInterpolate(toFill, x => 3 * x * x - 2 * x * x * x);
        }
    }

    public class CosineValueInterpolationGenerator : ValueInterpolationGenerator
    {
        public CosineValueInterpolationGenerator()
        {

        }

        public CosineValueInterpolationGenerator(int d, int u, int l) : base(d, u, l)
        { }

        public override double[][] Interpolate(double[][] toFill)
        {
            return BasicInterpolate(toFill, x => (1 - Math.Cos(Math.PI * x)) / 2);
        }
    }

    public abstract class ValueInterpolationGenerator : InterpolationMapGenerator
    {
        public ValueInterpolationGenerator(int d, int u, int l) : base(d,u,l)
        {
        }

        public ValueInterpolationGenerator() { }

        public abstract double[][] Interpolate(double[][] toFill);

        //should be array with 4 values on edges
        public static double[][] BasicInterpolate(double[][] arrayToFill, Func<double, double> s)
        {
            var xLength = arrayToFill.Length;
            var yLength = arrayToFill[0].Length;
            var z1 = arrayToFill[0][0];
            var z2 = arrayToFill[xLength - 1][0];
            var z3 = arrayToFill[0][yLength - 1];
            var z4 = arrayToFill[xLength - 1][yLength - 1];
            var xRange = Enumerable
                .Range(0, xLength);
            var yRange = Enumerable
                .Range(0, yLength);
            foreach (var x in xRange)
            {
                arrayToFill[x] = new double[yLength];
                foreach (var y in yRange)
                {
                    var fz1 = InterpolationStep_GetZ(z1, z2, 0, xLength - 1, x, s);
                    var fz2 = InterpolationStep_GetZ(z3, z4, 0, xLength - 1, x, s);
                    var z = InterpolationStep_GetZ(fz1, fz2, 0, yLength - 1, y, s);
                    arrayToFill[x][y] = z;
                }
            }
            return arrayToFill;
        }

        public override double[][] GetMap(int width, int height)
        {
            if (!DimensionIsOK(width, delta) || !DimensionIsOK(height, delta))
                throw new ArgumentException("You chosen wrong dimesions: (width-1) and (height-1)" +
                    " should be aliquot (delta -1)");
            var widthCount = GetLaticeCount(width, delta);
            var heightCount = GetLaticeCount(height, delta);
            var randomArray = MakeRandomArray(widthCount, heightCount,
                    lowerBound, upperBound);
            var res = new double[width][];
            int cur = 0;
            for (int x = 1; x < widthCount; x++)
            {
                var yLineDown = MakeEmptyArray(delta);
                for (int y = 1; y < heightCount; y++)
                {
                    var fourPointsArray = MakeFourPointsArray(delta, x, y, randomArray);
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
            //PrintArray(res);
            return res;
        }

        private static double[][] MakeFourPointsArray(int delta, int x, int y,
            double[,] randomArray)
        {
            var fourPointsArray = new double[delta][];
            for (int j = 0; j < delta; j++)
                fourPointsArray[j] = new double[delta];
            fourPointsArray[0][0] = randomArray[(x - 1), (y - 1)];
            fourPointsArray[0][delta - 1] = randomArray[(x - 1), y];
            fourPointsArray[delta - 1][0] = randomArray[x, y - 1];
            fourPointsArray[delta - 1][delta - 1] = randomArray[x, y];
            return fourPointsArray;
        }

        private static double[,] MakeRandomArray(int width, int height,
            int lBound, int uBound)
        {
            var rnd = new Random();
            var randomArray = new double[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    randomArray[x, y] = rnd.Next(lBound, uBound);
            return randomArray;
        }
    }
}
