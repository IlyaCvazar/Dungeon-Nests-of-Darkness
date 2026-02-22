using Godot;
using System;


public partial class WelcomeScript : Label
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	private void _on_mouse_entered()
	{
		VisibleCharacters = 0;
	}
	
	private void _on_timer_timeout()
	{
	VisibleCharacters += 1;
		
	} 
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//for (int i = 0; i < 11; i++){
			//VisibleRatio = (i / 10.0f);
		//}

	}
}
