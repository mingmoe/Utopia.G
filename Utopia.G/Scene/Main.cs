//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;
using System;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Translate;
using Utopia.G.Game;
using Utopia.G.Game.Player;
using Utopia.G.Graphy;
using Utopia.G.Scene;
using Utopia.Server.Net;

namespace Utopia.G;

public partial class Main : Node
{
    /// <summary>
    /// 在_Ready中初始化，视为非null。
    /// </summary>
    [GodotNodeBind("TileMap")]
    TileMap _map = null!;

    [GodotNodeBind("PlayerSprite")]
    Sprite2D _player = null!;

    [GodotNodeBind("Camera")]
    Camera2D _camera = null!;

    [GodotResourceBind("res://empty_default_tileset.tres")]
    TileSet _set = null!;

    /// <summary>
    /// 摄像机移动事件.
    /// 参数是既定的目标的摄像机位置(即玩家在TileMap中的坐标的全局坐标).
    /// 结果是将要移动到的摄像机位置.
    /// 时间不可以取消.
    /// </summary>
    public IEventManager<IEventWithParamAndResult<Vector2, Vector2>> CameraEvent
    { get; init; } = new EventManager<IEventWithParamAndResult<Vector2,Vector2>>();

    /// <summary>
    /// 在启动后被设置为非null
    /// </summary>
    public Core.IServiceProvider Service { get; private set; } = null!;

    public Game.Player.IPlayer Player { get; private set; } = null!;

    public override void _Ready()
    {
        GodotBinder.Bind(this, this);
        TileMapHelper.CreateTileMapLayer(this._map);

        var grass = ResourceLoader.Load<Texture2D>("res://images/textures/grass.png");

        var source = new TileSetAtlasSource
        {
            TextureRegionSize = new(32, 32),
            Texture = grass
        };
        source.CreateTile(new(0, 0), new(8, 8));

        _set.AddSource(source, 1);
        _set.TileSize = new Vector2I(32, 32);

        Random random = new();

        // _map.SetCell(0, new Vector2I(1, 1), 1, new Vector2I(0, 0));
        for (int x = -31; x != 32; x++)
        {
            for (int y = -31; y != 32; y++)
            {
                _map.SetCell(1, new Vector2I(x, y), 1, new Vector2I(
                    random.Next(0, 7),
                    random.Next(0, 7)));
            }
        }

        var server = Client.CreateLocalServer();
        this.Service = Client.Initlize(this);
        Client.Start(server, this.Service);

        this.Player = this.Service.GetService<Game.Player.IPlayer>();

        // set camera position
        this.Player.Position.Event.Register(
                (@event) =>
                {
                    var got = TileMapHelper.GetCameraPosition(this._map,
                        new Vector2I(@event.New.X,
                        @event.New.Y));

                    var cEvent = new ComplexEvent<Vector2, Vector2>(got, got, false);

                    this.CameraEvent.Fire(cEvent);

                    EventAssertionException.ThrowIfResultIsNull(cEvent);

                    this._camera.Position = cEvent.Result;
                }
            );
    }

    public override void _Process(double delta)
    {

    }
}
