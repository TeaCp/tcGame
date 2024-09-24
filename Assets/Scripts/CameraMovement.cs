using Godot;
using System;
using System.Security.AccessControl;

public partial class CameraMovement : Camera3D
{
	[Export] public NodePath PlayerNodePath;

	[Export] private float AccelerationSpeed;

	private Vector3 _originalPos;
	private CharacterBody3D _player;
	public override void _Ready()
	{
		_originalPos = Position;
		_player = GetNode<CharacterBody3D>(PlayerNodePath);
	}


	public override void _UnhandledInput(InputEvent @event)
	{
    	if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.T)
			{
				const float phi = Mathf.Pi / 4;
				Rotate(new Vector3(0, 1, 0), phi);
				_originalPos = _originalPos.Rotated(new Vector3(0, 1, 0), phi);
			}	
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Position = _player.Position + _originalPos;
		//LookAt(_player.Position);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
