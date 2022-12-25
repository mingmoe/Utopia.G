//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Godot;
using System;

namespace Utopia.G
{
	public partial class Menu : Node
	{
		Camera2D? camera;
		Viewport? Viewport;
		Sprite2D? background;
		Button? play;
		Button? exit;

		public override void _Ready()
		{
			if(Loading.player != null)
				this.AddChild(Loading.player);

			camera = GetNode<Camera2D>("Camera2D");

			Viewport = GetViewport();

			background = (Sprite2D)GetNode("Background");
			Utility.SetBackground(this, background);


			exit = (Button)GetNode("Control/GridContainer/CenterContainerExit/Exit");
			play = (Button)GetNode("Control/GridContainer/CenterContainerPlay/Play");

			exit.Pressed += () =>
			{
				GetTree().Quit();
			};

			play.Pressed += () =>
			{
				this.GetTree().ChangeSceneToFile("res://Main.tscn");
			};
		}


		public override void _Process(double delta)
		{


		}
	}
}
