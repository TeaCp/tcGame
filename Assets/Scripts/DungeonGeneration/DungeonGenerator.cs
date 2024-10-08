using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TeaCup.PixelGame.UtilTools;
using TeaCup.PixelGame.GameComponents;

namespace TeaCup.PixelGame.Dungeon;

public partial class DungeonGenerator : Node
{
    [Export] private int blocksCount;
    [Export] private int GenRadius;
    [Export] private int Dev;
    [Export] private int Mean;
    [Export] private int Seed = 0;

    private Random rnd;
    private Room[] rooms2d;
    private GridMap grid;

    private static int tileSize = 2;
    private List<Room> bigRooms = new(8);

    public override void _EnterTree()
    {
        GenerateLayout();
        GenerateDecorations();
        SpawnEnemies();
        GetNode<PlayerObject>("../Player").Spawn(new Vector3(bigRooms[0].CenterPoint.X, 1, bigRooms[0].CenterPoint.Y) * tileSize);
    }

    public void GenerateLayout()
    {
        grid = GetParent().GetChild<GridMap>(0);
        grid.Clear();
        tileSize = (int)grid.CellSize[0];

        if(Seed == 0)
            rnd = new((int)Time.GetTimeStringFromSystem().Hash());
        else
            rnd = new(Seed);

        rooms2d = new Room[blocksCount];

        for (int i  = 0; i < blocksCount; i++)
        {
            var room = new Room(Dev, Mean);
            room.Make(rnd, GenRadius);
            rooms2d[i] = room;
            if (room.Size.X > Mean * 1.3 && room.Size.Y > Mean * 1.3)
            {
                room.Color = GridColor.Room;
                bigRooms.Add(room);
            }
        }

        SeparateRooms();

        VisualizeRooms();

        CreateHallways();
    }

    public void VisualizeRooms()
    {
        foreach(var r in bigRooms)
            r.DrawOnGridMap(grid);
    }

    private void SeparateRooms()
    {
        do
        {
            for (int current = 0; current < blocksCount; current++)
            {
                for (int other = 0; other < blocksCount; other++)
                {
                    if (current == other || !rooms2d[current].IsOverlapped(rooms2d[other])) continue;

                    Vector2 dir = ((Vector2)rooms2d[other].CenterPoint - rooms2d[current].CenterPoint).Normalized();

                    rooms2d[current].Move(-dir);
                    rooms2d[other].Move(dir);
                }
            }
        }
        while (IsAnyRoomOverlapped());
    }

    private void CreateHallways() // youtu.be/h64U6j_sFgs
    {
        var del_graph = new AStar2D(); // triangulate
        var mst_graph = new AStar2D(); //Minimum spanning tree
        var rpv = new Vector2[0];
        for (int i = 0; i < bigRooms.Count; i++)
        {
            del_graph.AddPoint(i, bigRooms[i].CenterPoint);
            mst_graph.AddPoint(i, bigRooms[i].CenterPoint);
            rpv = rpv.Append(bigRooms[i].CenterPoint).ToArray();
        }

        var delaunay = new Queue<int>(Geometry2D.TriangulateDelaunay(rpv));
        var delaunayLenght = delaunay.Count / 3;
        for (int i = 0; i < delaunayLenght; i++)
        {
            var p1 = delaunay.Dequeue();
            var p2 = delaunay.Dequeue();
            var p3 = delaunay.Dequeue();
            del_graph.ConnectPoints(p1, p2);
            del_graph.ConnectPoints(p2, p3);
            del_graph.ConnectPoints(p1, p3);
        }

        var visited_points = new List<int>(bigRooms.Count);
        visited_points.Add(rnd.Next() % bigRooms.Count);
        while (visited_points.Count != mst_graph.GetPointCount())
        {
            var possible_connections = new List<Tuple<int, int>>(bigRooms.Count);
            foreach (var vp in visited_points)
            {
                foreach (var c in del_graph.GetPointConnections(vp))
                {
                    if (!visited_points.Contains((int)c))
                    {
                        possible_connections.Add(new Tuple<int, int>(vp, (int)c));
                    }
                }
            }
            var connection = rnd.GetItems(possible_connections.ToArray(), 1)[0];
            foreach (var pc in possible_connections)
            {
                if (rpv[pc.Item1].DistanceSquaredTo(rpv[pc.Item2]) <
                    rpv[connection.Item1].DistanceSquaredTo(rpv[connection.Item2]))
                {
                    connection = pc;
                }
            }
            visited_points.Add(connection.Item2);
            mst_graph.ConnectPoints(connection.Item1, connection.Item2);
            del_graph.DisconnectPoints(connection.Item1, connection.Item2);
        }

        AStar2D hallway_graph = mst_graph;
        for(int i = 0; i < del_graph.GetPointCount() * 0.12f; i++) // get back some connections to make loops
        {
            long p, c; short tries = 0;
            do
            {
                p = rnd.GetItems(del_graph.GetPointIds(), 1)[0];
                c = rnd.GetItems(del_graph.GetPointConnections(p), 1)[0];
            }
            while (hallway_graph.ArePointsConnected(p,c) && tries < del_graph.GetPointCount() * 2);
            if(!hallway_graph.ArePointsConnected(p, c) && c != p)
                hallway_graph.ConnectPoints(p, c);
        }
        DrawHallways(hallway_graph);
    }

    private void DrawHallways(AStar2D hallway_graph)
    {
        var hallways = new List<Tuple<Vector3, Vector3>>((int)hallway_graph.GetPointCount());
        foreach(var p in hallway_graph.GetPointIds()) // hallways maker
        {
            foreach(var c in hallway_graph.GetPointConnections(p))
            {
                if (c > p)
                {
                    var from = bigRooms[(int)p].GetNearest(bigRooms[(int)c].CenterPoint);
                    var from3 = Utils.V2ToV3(from);
                    var to = Utils.V2ToV3( bigRooms[(int)c].GetNearest(bigRooms[(int)p].CenterPoint) );
                    hallways.Add(new(from3, to));
                    grid.SetCellItem((Vector3I)from3, (int)GridColor.Door);
                    grid.SetCellItem((Vector3I)to, (int)GridColor.Door);
                }
            }
        }

        var astar = new AStarGrid2D()
        {
            Region = new Rect2I(-500, -500, 1000, 1000),
            DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never,
            DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan,
        };
        astar.Update();

        foreach(var t in grid.GetUsedCellsByItem((int)GridColor.Room))
            astar.SetPointSolid(new Vector2I(t.X,t.Z));

        foreach (var h in hallways)
        {
            var from = new Vector2I((int)h.Item1.X, (int)h.Item1.Z);
            var to = new Vector2I((int)h.Item2.X, (int)h.Item2.Z);
            var hall = new Coridor(astar.GetPointPath(from, to));

            for(int j = 0; j < rooms2d.Length; j++) //draw small rooms. REWORK THIS
            {
                if (hall.IsOverlapped(rooms2d[j]))
                {
                    rooms2d[j].DrawOnGridMap(grid);
                }
            }

            hall.DrawOnGridMap(grid);
        }
    }

    private void GenerateDecorations()
    {
        // Not implemented yet
    }
    private void SpawnEnemies()
    {
        // Not implemented yet
    }

    private bool IsAnyRoomOverlapped()
    {
        for (int current = 0; current < blocksCount; current++)
        {
            for (int other = 0; other < blocksCount; other++)
            {
                if (current != other && rooms2d[current].IsOverlapped(rooms2d[other])) return true;
            }
        }
        return false;
    }
}
