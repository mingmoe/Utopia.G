#region copyright
// This file(may named SimplexGenerator.cs) is a part of the project: Utopia.FastNoise.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.FastNoise.
//
// Utopia.FastNoise is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.FastNoise is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.FastNoise. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
        this._simplex = new FastNoise("Simplex");
        this._seed = seed;
    }

    public float Generate2D(int x, int y)
    {
        return this._simplex.GenSingle2D(x, y, this._seed);
    }

    public FastNoise.OutputMinMax Generate2DArray(out float[] outputs, int x, int y, int xSize, int ySize)
    {
        var opts = new float[x * y];

        var m = this._simplex.GenUniformGrid2D(opts, x, y, xSize, ySize, 8, this._seed);

        outputs = opts;
        return m;
    }
}
