#region copyright
// This file(may named Generator.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Map;
using Utopia.Server.Map;
using Utopia.Server.Plugin.Entity;

namespace Utopia.Server.Plugin.Map;

public class ClimateGenerator : IClimateGenerator
{
    public IElevation GetElevation(Position position)
    {
        throw new NotImplementedException();
    }

    public IPrecipitation GetPrecipitation(Position position)
    {
        throw new NotImplementedException();
    }

    public ITemperature GetTemperature(Position position)
    {
        throw new NotImplementedException();
    }
}

public class Generator : IWorldGenerator
{
    public void Generate(IAreaLayer area)
    {
        if (area.Stage == GenerationStage.Finish)
        {
            return;
        }

        AreaLayerBuilder areaLayer = new(area);

        if (area.Position.Z == IArea.GroundZ)
        {
            areaLayer.Fill(
                (b, i) =>
                {
                    return new GrassEntity();
                }
            );
        }
        else if (area.Position.Z > IArea.GroundZ)
        {
            // fill `AIR`
            return;
        }
        else
        {
            // TODO:FILL STONE
            areaLayer.Fill(
                (b, i) =>
                {
                    return new GrassEntity();
                }
            );
        }
    }
}
