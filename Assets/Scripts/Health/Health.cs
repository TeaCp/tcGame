using Godot;
using System;

public class Health
{
	private Node _node;
	private int _hp;

	public int HP => _hp;
	public event Action OnDeath;
	public event Action OnDamageRecieve;

	public Health(int hp, Node node)
	{
		this._hp = hp;
		this._node = node;
	}

	public void GetDamage(int dmg)
	{
		_hp -= dmg;
		OnDamageRecieve?.Invoke();
		if (_hp <= 0)
		{
			OnDeath?.Invoke();
			#if DEBUG // remove it
			GD.Print($"{_node.Name} is rip");
			#endif
		}
	}
	
}
