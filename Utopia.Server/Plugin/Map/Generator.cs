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

using Autofac;
using Utopia.Core.Collections;
using Utopia.Core.Map;
using Utopia.Core.Utilities;
using Utopia.Server.Entity;
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

    private readonly IEntityManager _entityManager;
    private readonly IContainer _container;

    public Generator(IEntityManager entityManager,IContainer container)
    {
        ArgumentNullException.ThrowIfNull(entityManager);
        ArgumentNullException.ThrowIfNull(container);
        this._entityManager = entityManager;
        this._container = container;
    }

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
                    return this._entityManager.Create(ResourcePack.Entity.GrassEntity.ID, null);
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
                    return this._entityManager.Create(ResourcePack.Entity.GrassEntity.ID, null);
                }
            );
        }
    }
}
