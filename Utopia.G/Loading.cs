//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;
using System.Diagnostics;
using Utopia.Core.Logging;

namespace Utopia.G;

public partial class Loading : Node2D
{
    readonly Stopwatch _stopwatch = new();
    CompressedTexture2D? _logo;
    CompressedTexture2D? _license_logo;
    bool _first_logo = true;
    bool _second_logo = true;
    Sprite2D? _image;
    Sprite2D? _background;
    PackedScene? _next;
    object _key = null!;

    public static Godot.AudioStreamPlayer? Player { get; set; }

    public override void _Ready()
    {
        Player = new();
        this.AddChild(Player);
        Player.Stream = (Godot.AudioStream)ResourceLoader.Load("res://music/main.ogg");
        Player.Play();
        _stopwatch.Start();

        _logo = (CompressedTexture2D)ResourceLoader.Load("res://images/utopia.png");
        _license_logo = (CompressedTexture2D)ResourceLoader.Load("res://images/license.png");
        _image = this.GetNode<Sprite2D>("Background/Image");
        _background = this.GetNode<Sprite2D>("Background");

        _key = Utility.SetBackground(this, _background);

        _next = ResourceLoader.Load<PackedScene>("res://Menu.tscn");

        LogManager.Init(true);
    }

    public override void _Process(double delta)
    {
        var mc = _stopwatch.ElapsedMilliseconds;

        // switch logo
        if (mc >= 500 && mc <= 3800 && _first_logo)
        {
            _image!.Texture = _logo;
            _first_logo = false;
        }
        else if (mc >= 3800 && mc <= 7050 && _second_logo)
        {
            _image!.Texture = _license_logo;
            _second_logo = false;
        }
        else if (mc > 7100 && !_first_logo && !_second_logo)
        {
            _image!.Texture = null;
        }

        // change scene after show logos
        if (mc > 7100)
        {
            this.RemoveChild(Player);
            Utility.CancelBackground(this, _key);
            this.GetTree().ChangeSceneToPacked(_next);
        }
    }
}
