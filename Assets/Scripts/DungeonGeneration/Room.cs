using Godot;
using System;
using System.Collections.Generic;
using TeaCup.PixelGame.UtilTools;

namespace TeaCup.PixelGame.Dungeon;

public class Room : DungeonTile
{
    private int MinSize = 4;
    private int MaxSize = 20;
    private Rect2I rect;

    public override Vector2I Position
    {
        get => rect.Position;
        set => rect.Position = value;
    }

    public Vector2I Size
    {
        get => rect.Size;
        set => rect.Size = value;
    }

    public Vector2I FarPoint => rect.End;// - Vector2.One;

    public Vector2I CenterPoint => Position + Size/2;

    public GridColor Color { get; set; } = GridColor.Hall;

    public Room(int minSize, int maxSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
    }
    public Room() { }

    public void Make(Random rnd, int genRadius)
	{
        Vector2 pos = Utils.GetRandomPointInCircle(genRadius, rnd).Floor();
        Vector2 size;
        //var mean = (MaxSize + MinSize) / 2; var dev = (MaxSize - MinSize) / 2;
        do
        {
            size = Utils.BoxMuller(MaxSize, MinSize, rnd).Floor();
        }
        while ((size.Abs().X / size.Abs().Y < 0.4 || size.Abs().X / size.Abs().Y > 3) || // not sausage-like
                (size.Abs().X / size.Abs().Y > 0.9 && size.Abs().X / size.Abs().Y < 1.1)); // not square-like

        rect = new Rect2I((Vector2I)pos, (Vector2I)size);
    }

    public override IEnumerator<Vector2I> GetEnumerator()
    {
        for (int i = (int)Position.X; i <= FarPoint.X; i++)
        {
            for (int j = (int)Position.Y; j <= FarPoint.Y; j++)
            {
                yield return new Vector2I (i,j);
            }
        }
    }

    public override Vector2I GetNearest(Vector2I point)
    {
        var nearest = Position;
        foreach(Vector2I p in this)
        {
            if (p.DistanceSquaredTo(point) < nearest.DistanceSquaredTo(point)) nearest = p;
        }
        return nearest;
    }

    public override void DrawOnGridMap(GridMap grid)
    {
        base.DrawOnGridMap(grid);

        foreach (var p in this)
        {
            var pos = new Vector3I(p.X,0,p.Y);
            var clr = (int)Color;

            DrawIfEmpty(grid, pos, clr);
        }
        for (int i = (int)Position.X - 1; i <= FarPoint.X + 1; i++)
        {
            var posUp = new Vector3I(i, 0, (int)Position.Y - 1);
            var posBot = new Vector3I(i, 0, (int)FarPoint.Y + 1);
            DrawIfEmpty(grid, posUp, (int)GridColor.Wall);
            DrawIfEmpty(grid, posBot, (int)GridColor.Wall);
        }
        for (int i = (int)Position.Y - 1; i <= FarPoint.Y + 1; i++)
        {
            var posUp = new Vector3I((int)Position.X - 1, 0, i);
            var posBot = new Vector3I((int)FarPoint.X + 1, 0, i);
            DrawIfEmpty(grid, posUp, (int)GridColor.Wall);
            DrawIfEmpty(grid, posBot, (int)GridColor.Wall);
        }
    }

    public override void Move(Vector2 move)
    {
        rect.Position += new Vector2I(
            Mathf.RoundToInt(move.X),
            Mathf.RoundToInt(move.Y)
        );
    }

    public override bool IsOverlapped(DungeonTile other)
    {
        if(other as Room != null)
            return rect.Grow(1).Intersects((other as Room).rect);
        return false; // TODO: rework this
    }
    public override bool HasPoint(in Vector2 p_point)
	{
		if (p_point.X < Position.X)
			return false;
		if (p_point.Y < Position.Y)
			return false;
		
		if (p_point.X > FarPoint.X )
			return false;
		if (p_point.Y > FarPoint.Y )
			return false;
		
		return true;
	}
}
