using Godot;
using System;

public partial class UIRender : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var fps_label = GetNode<Label>("FPSLabel");
		var fps_timer = fps_label.GetNode<Timer>("FPSTimer");
		fps_timer.Autostart = false;
		fps_timer.OneShot = false;
		fps_timer.Timeout += () => fps_label.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
		fps_timer.Start(1);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
