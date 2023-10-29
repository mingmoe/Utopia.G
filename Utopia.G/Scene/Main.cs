// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Godot;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.G.Game;
using Utopia.G.Graphy;
using Utopia.G.Scene;

namespace Utopia.G;

public partial class Main : Node
{
    /// <summary>
    /// 在_Ready中初始化，视为非null。
    /// </summary>
    [GodotNodeBind("TileMap")]
    private TileMap _map = null!;

    [GodotNodeBind("PlayerSprite")]
    private Sprite2D _player = null!;

    [GodotNodeBind("Camera")]
    private Camera2D _camera = null!;

    [GodotResourceBind("res://empty_default_tileset.tres")]
    private TileSet _set = null!;

    /// <summary>
    /// 摄像机移动事件.
    /// 参数是既定的目标的摄像机位置(即玩家在TileMap中的坐标的全局坐标).
    /// 结果是将要移动到的摄像机位置.
    /// 时间不可以取消.
    /// </summary>
    public IEventManager<IEventWithParamAndResult<Vector2, Vector2>> CameraEvent
    { get; init; } = new EventManager<IEventWithParamAndResult<Vector2, Vector2>>();

    /// <summary>
    /// 在启动后被设置为非null
    /// </summary>
    public Core.IServiceProvider Service { get; private set; } = null!;

    public Game.Player.IPlayer Player { get; private set; } = null!;

    public TileMap Map => _map;

    public Camera2D Camera => _camera;

    public override void _Ready()
    {
        GodotBinder.Bind(this, this);
        TileMapHelper.CreateTileMapLayer(_map);
        _set.TileSize = new Vector2I(32, 32);

        System.Uri server = Client.CreateLocalServer();
        Service = Client.Initlize(this);
        Client.Start(server, Service);

        // this.Player = this.Service.GetService<Game.Player.IPlayer>();

        // set camera position
        if (false)
        {
            Player.Position.Event.Register(
                    (@event) =>
                    {
                        Vector2 got = TileMapHelper.GetCameraPosition(_map,
                            new Vector2I(@event.New.X,
                            @event.New.Y));

                        var cEvent = new ComplexEvent<Vector2, Vector2>(got, got);

                        CameraEvent.Fire(cEvent);

                        _ = EventAssertionException.ThrowIfResultIsNull(cEvent);

                        _camera.Position = cEvent.Result;
                    }
                );
        }
    }

    public override void _Process(double delta)
    {

    }
}
