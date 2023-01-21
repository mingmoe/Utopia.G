//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;

namespace Utopia.G
{
    public partial class Main : Node
    {
        /// <summary>
        /// 在_Ready中初始化，视为非null。
        /// </summary>
        TileMap map = null!;

        Sprite2D player = null!;

        Camera2D camera = null!;

        TileSet set = null!;

        public override void _Ready()
        {
            map = (TileMap)this.FindChild("TileMap");
            player = (Sprite2D)this.FindChild("PlayerSprite");
            camera = (Camera2D)this.FindChild("Camera");
            set = map.TileSet;
        }


        public override void _Process(double delta)
        {




        }

    }
}
