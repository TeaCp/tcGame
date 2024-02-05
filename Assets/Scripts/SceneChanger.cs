using Godot;
using System;

public partial class SceneChanger : Node3D
{

	[Export] public NodePath SubViewPortPath;

	private SubViewport _subViewPort;
	// temporal
	private PackedScene[] _scenes;
	private int _sceneIdx = -1;
	
	public override void _Ready()
	{
		_subViewPort = GetNode<SubViewport>(SubViewPortPath);
		_scenes = new[]
		{
			GD.Load<PackedScene>("res://Assets/Scenes/home_scene.tscn"),
			GD.Load<PackedScene>("res://Assets/Scenes/demo_scene.tscn")
		};
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.O)
			{
				_sceneIdx = Mathf.Wrap(_sceneIdx + 1, 0, _scenes.Length);
				_subViewPort.GetChild<Node3D>(0).QueueFree();
				var instance = _scenes[_sceneIdx].Instantiate();
				_subViewPort.AddChild(instance);
			}
		}
	}
}
