using Godot;

public partial class CameraMovement : Camera3D
{
	public float CameraAngle => _initialAngle;

	[Export] private float _rotationDuration = 0.1f;

	[Export] private float _zoomDuration = 0.2f;

	[Export] private float _zoomSensitivity = 5.0f;

	[Export] private float _rotationSensitivity = 0.005f;

	[Export] private float _rotationStep = Mathf.Pi / 4;

	private float _initialAngle;

	private Basis _initialBasis;

	private Vector2 _initialCursorPosition;

	private Vector3 _localPosition;

	private PlayerScript _player;

	public override void _Ready()
	{
		_player = GetNode<PlayerScript>("../Player");
		if (_player is null) { GD.PrintErr("Failed to get Player's node for the Camera"); }
		
		_localPosition = Position - _player.Position;
		_initialBasis = _player.Basis;
		_initialAngle = _player.Basis.Z.SignedAngleTo(Plane.PlaneXZ.Project(_localPosition), Vector3.Up);

		Input.UseAccumulatedInput = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//? zoom handling
    	if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
    	{
			if(mouseButtonEvent.ButtonIndex is MouseButton.WheelUp or MouseButton.WheelDown)
			{
				//? take care of zoom orientation
				float signedZoomSensitivity = _zoomSensitivity * 
							((mouseButtonEvent.ButtonIndex == MouseButton.WheelUp) ? -1 : +1);
				
				//? extreme values handling
				float newCameraSize = Mathf.Clamp(Size + signedZoomSensitivity, 5.0f, 20.0f);

				Tween zoomTween = CreateTween().SetEase(Tween.EaseType.Out)
							.SetTrans(Tween.TransitionType.Circ);
				zoomTween.TweenProperty(this, "size", newCameraSize, _zoomDuration);
			}
    	}
		else if (@event is InputEventMouseMotion mouseMotionEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
		{	
			//? smooth camera rotation
			float rotationAngle = -mouseMotionEvent.ScreenRelative.X * _rotationSensitivity;
			Basis = new Basis(this.GetRotatedBasis(rotationAngle));

			//? smooth camera translation
			_localPosition = this.GetRotatedCamera(rotationAngle) * _localPosition;
		}
	}
	public override void _Input(InputEvent @event)
	{
		//TODO: add state machine for correct animations and zoom without interruption
		if (@event.IsActionPressed("rotate_camera_by_step_ccw"))
		{
			RotateCameraByStep(_rotationStep);
		}
		else if (@event.IsActionPressed("rotate_camera_by_step_cw"))
		{
			RotateCameraByStep(-_rotationStep);
		}
		else if (@event.IsActionPressed("rotate_camera_smoothly"))
		{
			_initialCursorPosition = GetViewport().GetMousePosition();
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (@event.IsActionReleased("rotate_camera_smoothly")) 
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetViewport().WarpMouse(_initialCursorPosition);
		} 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = _localPosition + _player.Position;
	}

	//? it assumes counterclockwise camera rotation
	private void RotateCameraByStep(float rotationStep)
	{
		
		//? create tween for smooth animation
		Tween cameraTween = CreateTween().SetEase(Tween.EaseType.In)
					.SetTrans(Tween.TransitionType.Sine).SetParallel(true);
		
		//? smooth camera rotation
		var currentBasisRotation = Basis.GetRotationQuaternion();
		var newBasisRotation = this.GetRotatedBasis(rotationStep);

		cameraTween.TweenMethod(Callable.From((float weight) => 
			Basis = new Basis(currentBasisRotation.Slerpni(newBasisRotation, weight))),
			0.0f, 1.0f, _rotationDuration);


		//? smooth camera translation
		var currentPosition = _localPosition;
		var currentLocalPositionRotation = _initialBasis.GetRotationQuaternion();
		var newLocalPositionRotation = this.GetRotatedCamera(rotationStep);

		cameraTween.TweenMethod(Callable.From((float weight) => 
			_localPosition = currentLocalPositionRotation.Slerpni(newLocalPositionRotation, weight)
			* currentPosition),	0.0f, 1.0f, _rotationDuration);
	}

	private Quaternion GetRotatedBasis(float angle)
	{
		return GetRotatedQuaternionY(Basis.GetRotationQuaternion(), angle);
	}

	private Quaternion GetRotatedCamera(float angle)
	{
		return GetRotatedQuaternionY(_initialBasis.GetRotationQuaternion(), angle);
	}

	private Quaternion GetRotatedQuaternionY(Quaternion quaternion, float angle)
	{
		return new Quaternion(Vector3.Up, angle) * quaternion;
	}
}
