using Godot;
using System;

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = _player.Position + _originalPos;
		LookAt(_player.Position);
	}
}
