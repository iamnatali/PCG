using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace PCG
{
    public abstract class InterpolationMapGenerator : HeightMapGenerator
    {
        public int delta { get; set; }
        public int lowerBound { get; set; }
        public int upperBound { get; set; }

        public InterpolationMapGenerator() { }

        public InterpolationMapGenerator(int d, int upper, int lower)
        {
            delta = d;
            upperBound = upper;
            lowerBound = lower;
            if (delta <= 0 || upperBound < lowerBound)
                throw new ArgumentException("wrong parameters");
        }

        public static double InterpolationStep_GetZ(double z1, double z2,
            double x1, double x2, double x, Func<double, double> s)
        {
            if (x2 <= x1)
                throw new ArgumentException("Wrong points orger given:n2 shoul be > n1");
            var weight1 = (x2 - x) / (x2 - x1);
            var weight2 = (x - x1) / (x2 - x1);
            return s(weight1) * z1 +
                s(weight2) * z2;
        }

        public static bool DimensionIsOK(int dimension, int delta)
        {
            return (dimension - 1) % (delta - 1) == 0;
        }

        public static int GetLaticeCount(int dimension, int delta)
        {
            return (dimension - 1) / (delta - 1) + 1;
        }

        public static void PrintArray(double[][] array)
        {
            for (int x = 0; x < array.Length; x++)
            {
                for (int y = 0; y < array[0].Length; y++)
                    Console.Write(array[x][y].ToString() + " ");
                Console.WriteLine();
            }
        }

        public static double[][] MakeEmptyArray(int firstLength)
        {
            var yLineDown = new double[firstLength][];
            for (int j = 0; j < firstLength; j++)
                yLineDown[j] = new double[0];
            return yLineDown;
        }

        public static double[][] AddYLine(double[][] yLineDown, double[][] fourPointsArray,
            Func<double[], IEnumerable<double>> f)
        {
            return yLineDown
                .Zip(fourPointsArray, (first, second) => first.Concat(f(second)).ToArray())
                .ToArray();
        }

        public abstract double[][] GetMap(int wigth, int height);
    }
}
