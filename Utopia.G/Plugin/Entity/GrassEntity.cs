// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Godot;
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
