using Godot;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;

public partial class CameraMovement : Camera3D
{
	[Export] private float animationDuration = 0.1f;

	[Export] private Vector3 _localPosition;

	private PlayerScript _player;

	public override void _Ready()
	{
		_player = GetNode<PlayerScript>("../Player");
		_localPosition = Position - _player.Position;
	}

	public override void _Input(InputEvent @event)
	{
		//TODO: add state machine for correct animation without interruption
		if (@event.IsActionPressed("rotate_camera") && true)
		{
				const float _phi = Mathf.Pi / 4;
				Vector3 _newLocalPosition = _localPosition.Rotated(Vector3.Up, _phi);

				Tween tween = CreateTween().SetParallel(true);

				//? camera rotation
				tween.TweenProperty(this, "basis", Basis.Rotated(Vector3.Up, _phi).Orthonormalized(), animationDuration);

				//? camera translation
				tween.TweenProperty(this, "_localPosition", _newLocalPosition, animationDuration);
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Position = _player.Position + _localPosition;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
