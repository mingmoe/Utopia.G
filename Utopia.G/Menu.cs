//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Godot;

namespace Utopia.G;

public partial class Menu : Node
{
    Camera2D? _camera;
    Viewport? _viewport;
    Sprite2D? _background;
    Button? _play;
    Button? _exit;

    public override void _Ready()
    {
        if (Loading.Player != null)
        {
            this.AddChild(Loading.Player);
        }

        _camera = this.GetNode<Camera2D>("Camera2D");

        _viewport = this.GetViewport();

        _background = (Sprite2D)this.GetNode("Background");
        Utility.SetBackground(this, _background);

        _exit = (Button)this.GetNode("Control/GridContainer/CenterContainerExit/Exit");
        _play = (Button)this.GetNode("Control/GridContainer/CenterContainerPlay/Play");

        _exit.Pressed += () =>
        {
            this.GetTree().Quit();
        };

        _play.Pressed += () =>
        {
            this.GetTree().ChangeSceneToFile("res://Main.tscn");
        };
    }

    public override void _Process(double delta)
    {
    }
}
