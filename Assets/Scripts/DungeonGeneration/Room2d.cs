using Godot;
using System;
using System.Collections.Generic;
using TeaCup.PixelGame.UtilTools;

public partial class Room2D
{
    private int MinSize = 4;
    private int MaxSize = 20;
    private Rect2 rect;

    public Vector2 Position
    {
        get => rect.Position;
        set => rect.Position = value;
    }

    public Vector2 Size
    {
        get => rect.Size;
        set => rect.Size = value;
    }

    public Vector2 FarPoint => rect.End;// - Vector2.One;

    public Vector2 CenterPoint => Position + Size/2;

    public GridColor Color { get; set; } = GridColor.Hall;

    public bool IsDrawn { get; private set; } = false;

    public Room2D(int minSize, int maxSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
    }
    public Room2D() { }

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

        rect = new Rect2(pos,size);
    }

    public IEnumerator<Vector2I> GetEnumerator()
    {
        for (int i = (int)Position.X; i <= FarPoint.X; i++)
        {
            for (int j = (int)Position.Y; j <= FarPoint.Y; j++)
            {
                yield return new Vector2I (i,j);
            }
        }
    }

    public Vector2 GetNearest(Vector2 point)
    {
        var nearest = Position;
        foreach(Vector2 p in this)
        {
            if (p.DistanceSquaredTo(point) < nearest.DistanceSquaredTo(point)) nearest = p;
        }
        return nearest;
    }

    public void DrawOnGridMap(GridMap grid)
    {
        if (IsDrawn) return;

        void DrawIfEmpty(Vector3I pos, int clr)
        {
            if (grid.GetCellItem(pos) == GridMap.InvalidCellItem)
            {
                grid.SetCellItem(pos, clr);
            }
        }

        foreach (var p in this)
        {
            var pos = new Vector3I(p.X,0,p.Y);
            var clr = (int)Color;

            DrawIfEmpty(pos, clr);
        }
        for (int i = (int)Position.X - 1; i <= FarPoint.X + 1; i++)
        {
            var posUp = new Vector3I(i, 0, (int)Position.Y - 1);
            var posBot = new Vector3I(i, 0, (int)FarPoint.Y + 1);
            DrawIfEmpty(posUp, (int)GridColor.Wall);
            DrawIfEmpty(posBot, (int)GridColor.Wall);
        }
        for (int i = (int)Position.Y - 1; i <= FarPoint.Y + 1; i++)
        {
            var posUp = new Vector3I((int)Position.X - 1, 0, i);
            var posBot = new Vector3I((int)FarPoint.X + 1, 0, i);
            DrawIfEmpty(posUp, (int)GridColor.Wall);
            DrawIfEmpty(posBot, (int)GridColor.Wall);
        }
        IsDrawn = true;
    }

    public void Move(Vector2 move, int tileSize = 1)
    {
        rect.Position += new Vector2(
            Mathf.RoundToInt(move.X) * tileSize,
            Mathf.RoundToInt(move.Y) * tileSize
        );
    }

    public bool IsOverlapped(Room2D other) => rect.Grow(1).Intersects(other.rect, true);

    public bool HasPoint(in Vector2 p_point)
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

    public enum GridColor
    {
        Corridor,
        Hall,
        Room,
        Door,
        Wall,
    }
}
