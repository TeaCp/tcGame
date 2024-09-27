using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public abstract class PlayerStateAlive : PlayerState
{
    protected float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    protected virtual float Speed => _player.Speed;

    public PlayerStateAlive(Fsm fsm, PlayerObject player) : base(fsm, player) { }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        var inputDir = GetInputDir();

        if (inputDir == Vector2.Zero)
        {
            _fsm.SetState<PlayerStateIdle>();
        }

        Move(inputDir, delta);
    }

    protected static Vector2 GetInputDir()
    {
        return Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    }

    public virtual void Move(Vector2 inputDirection, double delta)
    {
        const float rotateAngle = Mathf.Pi / 4;
        Vector3 direction = new Vector3(inputDirection.X, 0, inputDirection.Y).Normalized();
        Vector3 velocity = _player.Velocity;
        Vector3 rotation = _player.Rotation;

        if (!_player.IsOnFloor())
            velocity.Y -= _gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && _player.IsOnFloor())
            velocity.Y = _player.JumpVelocity;

        Vector2 inputDir = GetInputDir();

        if (direction != Vector3.Zero)
        {
            direction = direction.Rotated(Vector3.Up, rotateAngle);
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            rotation.Y = (float)Mathf.LerpAngle(rotation.Y, Mathf.Atan2(direction.X, direction.Z), delta * _player.RotateAccelerate);
        }
        else
        {
            velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(_player.Velocity.Z, 0, Speed);
        }

        _player.Velocity = velocity;
        _player.Rotation = rotation;
        _player.MoveAndSlide();
    }

    
}
