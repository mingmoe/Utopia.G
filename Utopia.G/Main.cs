//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;

namespace Utopia.G;

public partial class Main : Node
{
    /// <summary>
    /// 在_Ready中初始化，视为非null。
    /// </summary>
    TileMap _map = null!;

    Sprite2D _player = null!;

    Camera2D _camera = null!;

    TileSet _set = null!;

    public override void _Ready()
    {
        _map = (TileMap)this.FindChild("TileMap");
        _player = (Sprite2D)this.FindChild("PlayerSprite");
        _camera = (Camera2D)this.FindChild("Camera");
        _set = _map.TileSet;
    }

    public override void _Process(double delta)
    {

    }
}
