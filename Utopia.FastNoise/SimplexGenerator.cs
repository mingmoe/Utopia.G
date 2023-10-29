// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.FastNoise;

/// <summary>
/// Simplex噪音生成器
/// </summary>
public class SimplexGenerator : INoiseGenerator
{
    private readonly FastNoise _simplex;

#pragma warning disable IDE0044 // Add readonly modifier
    private int _seed = 0;
#pragma warning restore IDE0044 // Add readonly modifier

    public SimplexGenerator(int seed)
    {
        _simplex = new FastNoise("Simplex");
        _seed = seed;
    }

    public float Generate2D(int x, int y) => _simplex.GenSingle2D(x, y, _seed);

    public FastNoise.OutputMinMax Generate2DArray(out float[] outputs, int x, int y, int xSize, int ySize)
    {
        float[] opts = new float[x * y];

        FastNoise.OutputMinMax m = _simplex.GenUniformGrid2D(opts, x, y, xSize, ySize, 8, _seed);

        outputs = opts;
        return m;
    }
}
