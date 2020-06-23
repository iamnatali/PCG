using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG
{
    public interface HeightMapGenerator
    {
        double[][] GetMap(int wigth, int height);
    }
}
