using Godot;
using System;


public partial class WelcomeScript : RichTextLabel
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VisibleCharacters = 0;
	}
	private void _on_mouse_entered()
	{
		VisibleCharacters = 0;
		GD.Print("Time out " + GetTotalCharacterCount());
		Timer timer = GetNode<Timer>($"../Timer");
		timer.Start();
	}
	
	private void _on_timer_timeout()
	{
	if (VisibleCharacters < GetTotalCharacterCount())
	{
		VisibleCharacters += 1;
		GD.Print("Line_With: " + GetTotalCharacterCount());
		} 
		else
		{
			Timer timer = GetNode<Timer>($"../Timer");
			timer.Stop();
			GD.Print("He");
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//for (int i = 0; i < 11; i++){
			//VisibleRatio = (i / 10.0f);
		//}

	}
}
