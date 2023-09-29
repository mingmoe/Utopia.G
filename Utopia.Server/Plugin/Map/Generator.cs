using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Map;
using Utopia.Server.Map;

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
        if(area.Stage == GenerationStage.Finish)
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
