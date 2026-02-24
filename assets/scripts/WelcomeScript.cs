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
		if (VisibleCharacters == GetTotalCharacterCount())
			{
			VisibleCharacters = 0;
			Timer timer = GetNode<Timer>($"../Timer");
			timer.Start();
			}
		GD.Print("Total " + GetTotalCharacterCount());
		GD.Print("Total  chsr " + VisibleCharacters);
		
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
	//public override void _Process(double delta)
	//{
	//}
}
