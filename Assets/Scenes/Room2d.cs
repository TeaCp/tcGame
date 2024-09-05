using Godot;
using System;
using System.Collections.Generic;
using TeaCup.PixelGame.UtilTools;

public partial class Room2D
{
    private int MinSize = 4;
    private int MaxSize = 20;
    private int tileSize = 2;
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

    public Vector2 FarPoint => new(Position.X + Size.X, Position.Y + Size.Y);

    public Vector2 CenterPoint => Position + Size/2;

    public GridColor Color { get; set; } = GridColor.Blue;

    public bool IsDrawn { get; private set; } = false;

    public Room2D(int minSize, int maxSize, int tileSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
        this.tileSize = tileSize;
    }
    public Room2D() { }

    public void Make(Random rnd, int genRadius)
	{
        Vector2 pos = Utils.RoundM(Utils.GetRandomPointInCircle(genRadius, rnd), tileSize);
        Vector2 size;
        //var mean = (MaxSize + MinSize) / 2; var dev = (MaxSize - MinSize) / 2;
        do
        {
            size = Utils.RoundM(Utils.BoxMuller(MaxSize, MinSize, rnd), tileSize);
        }
        while ((size.Abs().X / size.Abs().Y < 0.4 || size.Abs().X / size.Abs().Y > 3) ||
                (size.Abs().X / size.Abs().Y > 0.9 && size.Abs().X / size.Abs().Y < 1.1));

        rect = new(pos,size);
    }

    public IEnumerator<Vector2I> GetEnumerator()
    {
        for (int i = (int)Position.X; i < FarPoint.X; i++)
        {
            for (int j = (int)Position.Y; j < FarPoint.Y; j++)
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

    public void DrawOnGridMap(GridMap grid) // todo: draw and find nearest 
    {
        if (IsDrawn) return;
        foreach (var p in this)
        {
            var pos = new Vector3I(p.X,0,p.Y);
            if (grid.GetCellItem(pos) == GridMap.InvalidCellItem)
            {
                grid.SetCellItem(pos, (int)Color);
            }
        }
        IsDrawn = true;
    }

    public void Move(Vector2 move, int tileSize = 2)
    {
        rect.Position += new Vector2(
            Mathf.RoundToInt(move.X) * tileSize,
            Mathf.RoundToInt(move.Y) * tileSize
        );
    }

    public bool IsOverlapped(Room2D other)
    {
        return rect.Intersects(other.rect, true);
        //var offset = Mesh.Size / 2;
        //var offsetOther = other.Mesh.Size / 2;
        //return !((Position.X - offset.X) > (other.Position.X + offsetOther.X) ||
        //       (other.Position.X - offsetOther.X) > (Position.X + offset.X) ||
        //       (Position.Y - offset.Y) > (other.Position.Y + offsetOther.Y) ||
        //       (other.Position.Y - offsetOther.Y) > (Position.Y + offset.Y));
    }

    public bool HasPoint(Vector2 point) => rect.HasPoint(point);

    public enum GridColor : byte
    {
        White,
        Blue,
        Red,
    }
}
