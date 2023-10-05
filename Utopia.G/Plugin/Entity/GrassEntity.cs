using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.G.Game.Entity;
using Utopia.G.Graphy;

namespace Utopia.G.Plugin.Entity;
public class GrassEntity : SimplayEntity
{
    public GrassEntity(Tile tile) : base(tile, ResourcePack.Entity.GrassEntity.ID)
    {
    }

    public override void FrameUpdate(double deltaSecond, Node node)
    {

    }
}
