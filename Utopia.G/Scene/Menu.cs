// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Godot;
using Utopia.G.Scene;

namespace Utopia.G;

public partial class Menu : Node
{
    private Camera2D? _camera;
    private Viewport? _viewport;
    private Sprite2D? _background;
    private Button? _play;
    private Button? _exit;
    private object _key = null!;

    public override void _Ready()
    {
        if (Loading.Player != null)
        {
            AddChild(Loading.Player);
        }

        _camera = GetNode<Camera2D>("Camera2D");

        _viewport = GetViewport();

        _background = (Sprite2D)GetNode("Background");
        _key = Utility.SetBackground(this, _background);

        _exit = (Button)GetNode("Control/GridContainer/CenterContainerExit/Exit");
        _play = (Button)GetNode("Control/GridContainer/CenterContainerPlay/Play");

        _exit.Pressed += () =>
        {
            Utility.CancelBackground(this, _key);
            GetTree().Quit();
        };

        _play.Pressed += () =>
        {
            Utility.CancelBackground(this, _key);
            _ = GetTree().ChangeSceneToFile("res://Main.tscn");
        };
    }

    public override void _Process(double delta)
    {
    }
}
