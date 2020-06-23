using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using PCGterrain;
using System.Windows.Forms;
using Ninject;
using Ninject.Extensions.Factory;
using System.Security.Policy;
using Ninject.Parameters;
using System.Reflection;
using System.IO;

namespace PCG
{
    class Program
    {
        //рефакторинг-все в одну функцию
        public static void NormalizeMap(double[][] map)
        {
            var max = map.Max(line => line.Max());
            for (int x = 0; x < map.Length; x++)
                for (int y = 0; y < map[0].Length; y++)
                    map[x][y] = (map[x][y] / max) * 255;
        }

        public static void MakeMapPositive(double[][] map)
        {
            var min = map.Min(line => line.Min());
            for (int x = 0; x < map.Length; x++)
                for (int y = 0; y < map[0].Length; y++)
                    map[x][y] = map[x][y] + Math.Abs(min);
        }

        public static void Dolinize(double[][] map)
        {
            var max = map.Max(line => line.Max());
            for (int x = 0; x < map.Length; x++)
                for (int y = 0; y < map[0].Length; y++)
                    map[x][y] = (map[x][y] / max) * (map[x][y] / max) * 255;
        }

        public static void SaveHeightMap(double[][] heightMap, int width, int height,
            string path)
        {
            var pic = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    var intHeight = (int)heightMap[x][y];
                    var color = Color.FromArgb(intHeight, intHeight, intHeight);
                    pic.SetPixel(x, y, color);
                }
            pic.Save(Path.Combine(path,"res.png"), pic.RawFormat);
            pic.Save(Path.Combine(path,"res.raw"), pic.RawFormat);
        }

        public static void GenerateHeightMapPic(HeightMapGenerator gen, int width,
            int height, bool dolinize, string path)
        {
            var heightMap = gen.GetMap(width, height);
            MakeMapPositive(heightMap);
            if (dolinize)
                Dolinize(heightMap);
            else
                NormalizeMap(heightMap);
            SaveHeightMap(heightMap, width, height, path);
        }

        [STAThread]
        static void Main(string[] args)
        {
            //var gen = new GenerationStatistics();
            //gen.GetStatistics();
            var form = new MenuForm();
            form.BackColor = Color.Lavender;
            form.Size = new Size(600, 600);
            form.StartPosition = FormStartPosition.CenterScreen;
            Application.Run(form);
        }
    }
}
