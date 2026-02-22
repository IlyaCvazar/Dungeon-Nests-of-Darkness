using Godot;
using System;

public partial class WDw : Button
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hi ready");
	}
	
	private void _on_pressed()
	{
	}
		public override void _Process(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update game logic here.
	}
}
