using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.FastNoise;

/// <summary>
/// Simplex噪音生成器
/// </summary>
public class SimplexGenerator : INoiseGenerator
{
    private readonly FastNoise _simplex;

    private int _seed = 0;

    public SimplexGenerator(int seed)
    {
        _simplex = new FastNoise("Simplex");
        this._seed = seed;
    }

    public float Generate2D(int x, int y)
    {
        return _simplex.GenSingle2D(x, y, this._seed);
    }

    public FastNoise.OutputMinMax Generate2DArray(out float[] outputs,int x,int y,int xSize,int ySize)
    {
        var opts = new float[x * y];
        
        var m = _simplex.GenUniformGrid2D(opts, x, y, xSize, ySize, 8, this._seed);

        outputs = opts;
        return m;
    }
}
