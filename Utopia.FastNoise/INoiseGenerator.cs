// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.FastNoise;

/// <summary>
/// 噪音生成器
/// </summary>
public interface INoiseGenerator
{
    float Generate2D(int x, int y);

    FastNoise.OutputMinMax Generate2DArray(out float[] outputs, int x, int y, int xSize, int ySize);
}
