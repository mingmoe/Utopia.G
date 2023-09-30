#region copyright
// This file(may named Menu.cs) is a part of the project: Utopia.G.
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
using Utopia.G.Scene;

namespace Utopia.G;

public partial class Menu : Node
{
    Camera2D? _camera;
    Viewport? _viewport;
    Sprite2D? _background;
    Button? _play;
    Button? _exit;
    object _key = null!;

    public override void _Ready()
    {
        if (Loading.Player != null)
        {
            this.AddChild(Loading.Player);
        }

        this._camera = this.GetNode<Camera2D>("Camera2D");

        this._viewport = this.GetViewport();

        this._background = (Sprite2D)this.GetNode("Background");
        this._key = Utility.SetBackground(this, this._background);

        this._exit = (Button)this.GetNode("Control/GridContainer/CenterContainerExit/Exit");
        this._play = (Button)this.GetNode("Control/GridContainer/CenterContainerPlay/Play");

        this._exit.Pressed += () =>
        {
            Utility.CancelBackground(this, this._key);
            this.GetTree().Quit();
        };

        this._play.Pressed += () =>
        {
            Utility.CancelBackground(this, this._key);
            this.GetTree().ChangeSceneToFile("res://Main.tscn");
        };
    }

    public override void _Process(double delta)
    {
    }
}
