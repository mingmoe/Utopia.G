//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;
using System.Diagnostics;
using Utopia.Core;

namespace Utopia.G
{
    public partial class Loading : Node2D
    {
        readonly Stopwatch stopwatch = new();
        CompressedTexture2D? logo;
        CompressedTexture2D? license_logo;
        bool first_logo = true;
        bool second_logo = true;
        Sprite2D? image;
        Sprite2D? background;
        PackedScene? next;

        public static Godot.AudioStreamPlayer? player;

        public override void _Ready()
        {
            player = new();
            this.AddChild(player);
            player.Stream = (Godot.AudioStream)ResourceLoader.Load("res://music/main.ogg");
            player.Play();
            stopwatch.Start();

            logo = (CompressedTexture2D)ResourceLoader.Load("res://images/utopia.png");
            license_logo = (CompressedTexture2D)ResourceLoader.Load("res://images/agplv3.png");
            image = GetNode<Sprite2D>("Background/Image");
            background = GetNode<Sprite2D>("Background");

            Utility.SetBackground(this, background);

            next = ResourceLoader.Load<PackedScene>("res://Menu.tscn");

            LogManager.Init(true);
        }

        public override void _Process(double delta)
        {

            var mc = stopwatch.ElapsedMilliseconds;

            // switch logo
            if (mc >= 500 && mc <= 3800 && first_logo)
            {
                image!.Texture = logo;
                first_logo = false;
            }
            else if (mc >= 3800 && mc <= 7050 && second_logo)
            {
                image!.Texture = license_logo;
                image.Scale = new Vector2(0.002f, 0.003f);
                second_logo = false;
            }
            else if (mc > 7100 && !first_logo && !second_logo)
            {
                image!.Texture = null;
            }

            // change scene after show logos
            if (mc > 7100)
            {
                this.RemoveChild(player);
                this.GetTree().ChangeSceneToPacked(next);
            }
        }
    }
}
