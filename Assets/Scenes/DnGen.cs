using Godot;
using System;

public partial class DnGen : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] private int blocksCount;
	[Export] private int GenRadius;
	[Export] private PackedScene blockPrefab;
	private Node3D[] blocksArray;
	private Random rnd;

	public override void _EnterTree()
	{
		blocksArray = new Node3D[blocksCount];
		rnd = new((int)Time.GetTimeStringFromSystem().Hash());

		for(int i = 0; i < blocksCount; i++)
		{
			var block = blockPrefab.Instantiate<Node3D>();
			var shape = new BoxShape3D();
			shape.Size = new Vector3(rnd.Next(4,20),1,rnd.Next(4,20));
			block.GetChild<CollisionShape3D>(0).Shape = shape;

			block.Position = new Vector3(0, 0, 0);

			//var point = GetRandomPointInCircle(GenRadius);
			//block.Position = new Vector3(point.X, 0, point.Y);
			//GD.Print(block.Position);
            
            AddChild(block);
            blocksArray[i] = block;
		}
		
		//for(int i = 0; i < blocksCount; i++)
		//{
		//	(blocksArray[i] as RigidBody3D).Freeze = false;
		//}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//for (int i = 0; i < blocksCount; i++)
		//{
		//	(blocksArray[i] as RigidBody3D).MoveAndCollide(Vector3.Zero);
		//}
	}

	private float RoundM(double n, double m) => (float)Math.Floor(((n+m - 1)/m)*m);

	private int tileSize = 4;
	private Vector2 GetRandomPointInCircle(int radius)
	{
		var t = 2 * Math.PI * rnd.NextDouble();
		var u = rnd.NextDouble() + rnd.NextDouble();
		double r;
		if(u>1) r = 2-u;
		else r = u;
		return new Vector2(RoundM(radius*r*Math.Cos(t), tileSize), RoundM(radius*r*Math.Sin(t), tileSize));
	}
}
