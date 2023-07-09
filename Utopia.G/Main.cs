//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;
using Utopia.Core.Translate;

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
        _set = new TileSet();
        _map.TileSet = _set;

        var grass = ResourceLoader.Load<Texture2D>("res://images/textures/grass.png");

        var source = new TileSetAtlasSource
        {
            Texture = grass,
            TextureRegionSize = new(64, 64),
            ResourceName = "grass"
        };
        source.CreateTile(new Vector2I(0, 0), new Vector2I(1, 1));

        _set.AddSource(source, 1);

        // _map.SetCell(0, new Vector2I(1, 1), 1, new Vector2I(0, 0));
        for (int x = 0; x != 32; x++)
        {
            for (int y = 0; y != 32; y++)
            {
                _map.SetCell(0, new Vector2I(x, y), 1, new Vector2I(0, 0));
            }
        }

        var server = Client.CreateLocalServer();
        Client.Start(server);
    }

    public override void _Process(double delta)
    {
    }
}
