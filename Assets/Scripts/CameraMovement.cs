using Godot;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;

public partial class CameraMovement : Camera3D
{
	[Export] private float _rotationDuration = 0.2f;

	[Export] private float _zoomDuration = 0.2f;

	[Export] private float _currentAngle;

	[Export] private float _zoomSensitivity = 5.0f;

	private Vector3 _localPosition;

	private float _localRadius;

	private PlayerScript _player;

	public override void _Ready()
	{
		_player = GetNode<PlayerScript>("../Player");
		_localPosition = Position - _player.Position;
		_localRadius = Mathf.Sqrt(_localPosition.X * _localPosition.X + _localPosition.Y * _localPosition.Y);
		_currentAngle =  Rotation.Y;
		//Input.UseAccumulatedInput = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
    	{
			if(mouseEvent.ButtonIndex == MouseButton.WheelUp || 
				mouseEvent.ButtonIndex == MouseButton.WheelDown)
			{
				//? take care of zoom orientation
				float _signedZoomSensitivity = _zoomSensitivity * 
							((mouseEvent.ButtonIndex == MouseButton.WheelUp) ? -1 : +1);
				
				//? extreme values handling
				float _newSize = Mathf.Clamp(Size + _signedZoomSensitivity, 5.0f, 20.0f);

				Tween _tween = CreateTween().SetEase(Tween.EaseType.Out)
							.SetTrans(Tween.TransitionType.Circ);
				_tween.TweenProperty(this, "size", _newSize, _zoomDuration);
			}
    	}
	}
	public override void _Input(InputEvent @event)
	{
		//TODO: add state machine for correct animation without interruption
		if (@event.IsActionPressed("rotate_camera"))
		{
			const float _phi = Mathf.Pi / 4;

			Tween _tween = CreateTween().SetEase(Tween.EaseType.Out)
							.SetTrans(Tween.TransitionType.Cubic).SetParallel(true);
			
			//? smooth camera rotation
			var _currentRotation = Basis.GetRotationQuaternion();
			var _newRotation = (new Quaternion(Vector3.Up, _phi) * _currentRotation).Normalized();

			_tween.TweenMethod(Callable.From((float weight) => 
				Basis = new Basis(_currentRotation.Slerpni(_newRotation, weight).Normalized())),
				0.0f, 1.0f, _rotationDuration);

			//? smooth camera translation
			_tween.TweenProperty(this, "_currentAngle", -_phi, _rotationDuration).AsRelative();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = new Vector3(_player.Position.X + _localRadius*Mathf.Cos(_currentAngle),
							_player.Position.Y + _localPosition.Y,
							_player.Position.Z + _localRadius*Mathf.Sin(_currentAngle));
	}
}
