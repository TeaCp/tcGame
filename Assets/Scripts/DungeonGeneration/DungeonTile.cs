using Godot;
using System.Collections.Generic;

namespace TeaCup.PixelGame.Dungeon;

public abstract class DungeonTile
{
    public abstract Vector2I Position { get; set; }

    public GridColor Color { get; set; } = GridColor.Hall;

    public bool IsDrawn { get; private set; } = false;

    public abstract IEnumerator<Vector2I> GetEnumerator();

    public virtual Vector2I GetNearest(Vector2I point)
    {
        var nearest = Position;
        foreach (Vector2I p in this)
        {
            if (p.DistanceSquaredTo(point) < nearest.DistanceSquaredTo(point)) nearest = p;
        }
        return nearest;
    }

    protected void DrawIfEmpty(GridMap grid, Vector3I pos, int clr)
    {
        if (grid.GetCellItem(pos) == GridMap.InvalidCellItem)
        {
            grid.SetCellItem(pos, clr);
        }
    }

    public virtual void DrawOnGridMap(GridMap grid)
    {
        if (IsDrawn) return;

        IsDrawn = true;
    }

    public abstract void Move(Vector2 move);

    public abstract bool IsOverlapped(DungeonTile other);

    public abstract bool HasPoint(in Vector2 p_point);
}
public enum GridColor
{
Corridor,
Hall,
Room,
Door,
Wall,
}
