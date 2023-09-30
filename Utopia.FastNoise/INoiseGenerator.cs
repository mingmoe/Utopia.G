#region copyright
// This file(may named INoiseGenerator.cs) is a part of the project: Utopia.FastNoise.
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
/// 噪音生成器
/// </summary>
public interface INoiseGenerator
{
    float Generate2D(int x, int y);

    FastNoise.OutputMinMax Generate2DArray(out float[] outputs, int x, int y, int xSize, int ySize);
}
