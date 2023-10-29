// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Diagnostics;
using Godot;
using Utopia.Core.Logging;
using Utopia.G.Scene;

namespace Utopia.G;

public partial class Loading : Node2D
{
    private readonly Stopwatch _stopwatch = new();
    private CompressedTexture2D? _logo;
    private CompressedTexture2D? _license_logo;
    private bool _first_logo = true;
    private bool _second_logo = true;
    private Sprite2D? _image;
    private Sprite2D? _background;
    private PackedScene? _next;
    private object _key = null!;

    public static Godot.AudioStreamPlayer? Player { get; set; }

    public override void _Ready()
    {
        Player = new();
        AddChild(Player);
        Player.Stream = (Godot.AudioStream)ResourceLoader.Load("res://music/main.ogg");
        Player.Play();
        _stopwatch.Start();

        _logo = (CompressedTexture2D)ResourceLoader.Load("res://images/utopia.png");
        _license_logo = (CompressedTexture2D)ResourceLoader.Load("res://images/license.png");
        _image = GetNode<Sprite2D>("Background/Image");
        _background = GetNode<Sprite2D>("Background");

        _key = Utility.SetBackground(this, _background);

        _next = ResourceLoader.Load<PackedScene>("res://Menu.tscn");

        LogManager.Init(LogManager.LogOption.CreateBatch());
    }

    public override void _Process(double delta)
    {
        long mc = _stopwatch.ElapsedMilliseconds;

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
            RemoveChild(Player);
            Utility.CancelBackground(this, _key);
            _ = GetTree().ChangeSceneToPacked(_next);
        }
    }
}
