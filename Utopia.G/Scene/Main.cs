#region copyright
// This file(may named Main.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Godot;
using System;
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
    { get; init; } = new EventManager<IEventWithParamAndResult<Vector2, Vector2>>();

    /// <summary>
    /// 在启动后被设置为非null
    /// </summary>
    public Core.IServiceProvider Service { get; private set; } = null!;

    public Game.Player.IPlayer Player { get; private set; } = null!;

    public TileMap Map => _map;

    public Camera2D Camera=> _camera;

    public override void _Ready()
    {
        GodotBinder.Bind(this, this);
        TileMapHelper.CreateTileMapLayer(this._map);
        this._set.TileSize = new Vector2I(32, 32);


        var server = Client.CreateLocalServer();
        this.Service = Client.Initlize(this);
        Client.Start(server, this.Service);

        // this.Player = this.Service.GetService<Game.Player.IPlayer>();

        // set camera position
        if(false)
        this.Player.Position.Event.Register(
                (@event) =>
                {
                    var got = TileMapHelper.GetCameraPosition(this._map,
                        new Vector2I(@event.New.X,
                        @event.New.Y));

                    var cEvent = new ComplexEvent<Vector2, Vector2>(got, got);

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
