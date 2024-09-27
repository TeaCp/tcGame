using System;
using System.Collections.Generic;
using Godot;
using TeaCup.PixelGame.UtilTools;

namespace TeaCup.PixelGame.Dungeon;

public class Coridor : DungeonTile
{
    public Vector2I[] Tiles { get; private set; }
    public override Vector2I Position
    {
        get => Tiles[0];
        set
        {
            var diff = Tiles[0] - value;
            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = Tiles[i] + diff;
            }
        }
    }

    public Coridor(Vector2I[] tiles)
    {
        Tiles = tiles;
        Color = GridColor.Corridor;
    }
    public Coridor(Vector2[] tiles)
    {
        var tilesI = new Vector2I[tiles.Length];
        for(int i = 0; i < tiles.Length; i++)
        {
            tilesI[i] = (Vector2I)tiles[i];
        }
        Tiles = tilesI;
        Color = GridColor.Corridor;
    }

    public override void DrawOnGridMap(GridMap grid)
    {
        foreach(var t in this)
        {
            var pos = (Vector3I)Utils.V2ToV3(t);
            if (grid.GetCellItem(pos) == GridMap.InvalidCellItem ||
                    grid.GetCellItem(pos) == (int)GridColor.Wall)
            {
                for (int j = pos.X - 1; j <= pos.X + 1; j++) // draw walls. TODO: work on it
                {
                    for (int k = pos.Z - 1; k <= pos.Z + 1; k++)
                    {
                        if (j != pos.X && k != pos.Z)
                            DrawIfEmpty(grid, new Vector3I(j, 0, k), (int)GridColor.Wall);
                    }
                }
                grid.SetCellItem(pos, (int)Color);
            }
        }
    }

    public override IEnumerator<Vector2I> GetEnumerator()
    {
        foreach (var tile in Tiles) { yield return tile; }
    }

    public override void Move(Vector2 move)
    {
        Position += new Vector2I(
            Mathf.RoundToInt(move.X),
            Mathf.RoundToInt(move.Y)
        );
    }

    [Obsolete("IsOverlapped method for Coridor is very expensive and not recommended to use")]
#pragma warning disable CS0809
    public override bool IsOverlapped(DungeonTile other)
#pragma warning restore CS0809 
    {
        #warning "IsOverlapped method for Coridor is very expensive and not recommended to use"
        foreach(var t in this)
        {
            foreach(var o in other)
            {
                if(o == t) return true;
            }
        }
        return false;
    }

    public override bool HasPoint(in Vector2 p_point)
    {
        foreach(var t in this)
        {
            if(t == p_point) return true;
        }
        return false;
    }
}
