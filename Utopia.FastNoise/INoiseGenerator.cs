using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.FastNoise;

/// <summary>
/// 噪音生成器
/// </summary>
public interface INoiseGenerator
{
    float Generate2D(int x,int y);

    FastNoise.OutputMinMax Generate2DArray(out float[] outputs, int x, int y, int xSize, int ySize);
}
