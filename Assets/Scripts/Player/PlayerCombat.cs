using Godot;

namespace TeaCup.PixelGame.GameComponents;

public partial class PlayerObject
{
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventKey)
		{
			if (eventKey.Pressed && eventKey.ButtonIndex == MouseButton.Left)
			{
				CollisionShape3D shape = _hitArea.GetChild<CollisionShape3D>(0);
				shape.Disabled = false;
				_animator.CurrentAnimation = "Punch";
				GetTree().CreateTimer(_animator.CurrentAnimationLength).Timeout +=
					() =>
					{
						_animator.CurrentAnimation = "Idle";
						shape.Disabled = true;
					};
			}
		}
	}

	private void DealDamageOnAreaEntered(Node3D obj)
	{
		if (!obj.IsInGroup("DamageReceivable")) return;
		IDamageReceivable damageReceivable = obj as IDamageReceivable;
		damageReceivable.HP.GetDamage(1);
	}
}
