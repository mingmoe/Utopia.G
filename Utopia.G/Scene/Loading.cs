#region copyright
// This file(may named Loading.cs) is a part of the project: Utopia.G.
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
using System.Diagnostics;
using Utopia.Core.Logging;
using Utopia.G.Scene;

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
        this._stopwatch.Start();

        this._logo = (CompressedTexture2D)ResourceLoader.Load("res://images/utopia.png");
        this._license_logo = (CompressedTexture2D)ResourceLoader.Load("res://images/license.png");
        this._image = this.GetNode<Sprite2D>("Background/Image");
        this._background = this.GetNode<Sprite2D>("Background");

        this._key = Utility.SetBackground(this, this._background);

        this._next = ResourceLoader.Load<PackedScene>("res://Menu.tscn");

        LogManager.Init(LogManager.LogOption.CreateBatch());
    }

    public override void _Process(double delta)
    {
        var mc = this._stopwatch.ElapsedMilliseconds;

        // switch logo
        if (mc >= 500 && mc <= 3800 && this._first_logo)
        {
            this._image!.Texture = this._logo;
            this._first_logo = false;
        }
        else if (mc >= 3800 && mc <= 7050 && this._second_logo)
        {
            this._image!.Texture = this._license_logo;
            this._second_logo = false;
        }
        else if (mc > 7100 && !this._first_logo && !this._second_logo)
        {
            this._image!.Texture = null;
        }

        // change scene after show logos
        if (mc > 7100)
        {
            this.RemoveChild(Player);
            Utility.CancelBackground(this, this._key);
            this.GetTree().ChangeSceneToPacked(this._next);
        }
    }
}
