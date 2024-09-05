using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;

public partial class dgen2d : Node
{
    [Export] private int blocksCount;
    [Export] private int GenRadius;
    [Export] private int Dev;
    [Export] private int Mean;
    [Export] private int Seed = 0;

    private Random rnd;
    private Room2D[] rooms2d;
    private GridMap grid;

    private static int tileSize = 2;
    private List<Room2D> bigRooms = new(8);

    public override void _EnterTree()
    {
        base._EnterTree();
        grid = GetParent().GetChild<GridMap>(0);
        grid.Clear();
        tileSize = (int)grid.CellSize[0];

        if(Seed == 0)
            rnd = new((int)Time.GetTimeStringFromSystem().Hash());
        else
            rnd = new(Seed);

        rooms2d = new Room2D[blocksCount];

        for (int i  = 0; i < blocksCount; i++)
        {
            var room = new Room2D(Dev, Mean, tileSize);
            room.Make(rnd, GenRadius);
            rooms2d[i] = room;
            if (room.Size.X > Mean * 1.35 && room.Size.Y > Mean * 1.35)
            {
                room.Color = Room2D.GridColor.Red;
                bigRooms.Add(room);
            }
        }

        SeparateRooms();
        var tr = Triangulate(bigRooms);
        var tree = FindMinimumSpanningTree(tr);

        VisualizeRooms(tree);
        DrawLines(tree);

        // set char position
        GetParent().GetChild<Node3D>(1).Position = new Vector3(bigRooms[0].CenterPoint.X, 1.5f, bigRooms[0].CenterPoint.Y) * tileSize; 
    }

    private void DrawTriangles(TriangleNet.Mesh tr)
    {
        foreach (var t in tr.Triangles)
        {
            var verts = new Vector3[]
            {
                    new Vector3((float)t.GetVertex(0).X, 2, (float)t.GetVertex(0).Y),
                    new Vector3((float)t.GetVertex(1).X, 2, (float)t.GetVertex(1).Y),
                    new Vector3((float)t.GetVertex(2).X, 2, (float)t.GetVertex(2).Y)
            };

            var obj = new MeshInstance3D();
            var mesh = new ArrayMesh();
            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = verts;
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
            var mat = new StandardMaterial3D(); mat.AlbedoColor = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), 0.3f);
            mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            mesh.SurfaceSetMaterial(0, mat);
            obj.Mesh = mesh;
            AddChild(obj);
        }
    }

    private void DrawLines(Edge[] tree)
    {
        foreach (var e in tree)
        {
            var verts = new Vector3[]
            {
                    new Vector3((float)e.P1.X, 2, (float)e.P1.Y),
                    new Vector3((float)e.P2.X, 2, (float)e.P2.Y)
            };

            var obj = new MeshInstance3D();
            var mesh = new ArrayMesh();
            var arrays = new Godot.Collections.Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = verts;
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

            var mat = new StandardMaterial3D(); mat.AlbedoColor = new Color(0, 255, 255, 0.6f);
            mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            mesh.SurfaceSetMaterial(0, mat);
            obj.Mesh = mesh;
            AddChild(obj);
        }
    }

    public void VisualizeRooms(Edge[] edges)
    {
        var astar = new AStarGrid2D();
        astar.Region = new Rect2I(-500,-500,1000,1000);
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
        astar.CellSize = Vector2.One * tileSize;
        astar.Update();

        void DrawRoom(Room2D room, bool solid = true)
        {
            foreach(var t in room)
            {
                var pos = new Vector3I(t.X, 0, t.Y);
                if (grid.GetCellItem(pos) == GridMap.InvalidCellItem)
                {
                    grid.SetCellItem(pos, (int)room.Color);
                    if (solid) astar.SetPointSolid(t);
                }
            }
        }

        foreach (var e in edges) // draw coridors
        {
            var r1 = bigRooms[e.P1.ID]; var r2 = bigRooms[e.P2.ID];
            var pos_from = r1.GetNearest(r2.CenterPoint);
            var pos_to = r2.GetNearest(r1.CenterPoint);

            grid.SetCellItem(new Vector3I((int)pos_from.X, 0, (int)pos_from.Y), 3);
            grid.SetCellItem(new Vector3I((int)pos_to.X, 0, (int)pos_to.Y), 3);
            DrawRoom(r1); DrawRoom(r2);

            astar.SetPointSolid((Vector2I)pos_from, false); astar.SetPointSolid((Vector2I)pos_to, false);
            var hall = astar.GetIdPath((Vector2I)pos_from, (Vector2I)pos_to, true);
            foreach (var h in hall)
            {
                foreach(var r in rooms2d) //draw small rooms
                {
                    if (r.HasPoint(h))
                    {
                        DrawRoom(r, false);
                    }
                }

                var pos = new Vector3I((int)h.X, 0, (int)h.Y);
                if(grid.GetCellItem(pos) == GridMap.InvalidCellItem)
                {
                    grid.SetCellItem(pos,0);
                }
            }
        }
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

                    var dir = (rooms2d[other].CenterPoint - rooms2d[current].CenterPoint).Normalized();

                    rooms2d[current].Move(-dir);
                    rooms2d[other].Move(dir);
                }
            }
        }
        while (IsAnyRoomOverlapped());
    }

    private TriangleNet.Mesh Triangulate(List<Room2D> rooms)
    {
        List<Vertex> vertices = new List<Vertex>();
        for(int i = 0;i<rooms.Count;i++)
        {
            vertices.Add(new Vertex(rooms[i].CenterPoint.X, rooms[i].CenterPoint.Y, i));
        }
        var tring = new TriangleNet.Meshing.Algorithm.SweepLine();
        var t = tring.Triangulate(vertices, new TriangleNet.Configuration());
        
        return (TriangleNet.Mesh)t;
    }

    public class Edge(Vertex p1, Vertex p2, float weight) : IComparable<Edge>
    {
        internal Vertex P1 { get; } = p1;
        internal Vertex P2 { get; } = p2;
        private float _weight = weight;

        public int CompareTo(Edge other)
        {
            if (other == null) return 1;
            return _weight.CompareTo(other._weight);
        }
    }

    private Edge[] FindMinimumSpanningTree(TriangleNet.Mesh graph)
    {
        List<Edge> gr1 = new(18);

        var grEdges = graph.Edges.ToArray();
        var grVert = graph.Vertices.ToArray();
        for(int i = 0; i< graph.NumberOfEdges;i++)
        {
            var p1 = grVert[grEdges[i].P0];
            var p2 = grVert[grEdges[i].P1];
            gr1.Add(new(p1, p2, (float)Mathf.Sqrt(Mathf.Pow(p2.X-p1.X,2) + Mathf.Pow(p2.Y - p1.Y, 2))));
        }
        gr1.Sort();

        var disjointSets = new SystemOfDisjointSets();
        foreach (Edge edge in gr1)
        {
            disjointSets.AddEdgeInSet(edge);
        }

        var rawRes = disjointSets.Sets.First().SetGraph;
        List<Rect2> result = new(18);

        for(int i = 0; i < gr1.Count * 0.1; i++)
        {
            Edge add; short tries = 0;
            do
            {
                add = rnd.GetItems(gr1.ToArray(), 1)[0];
                tries++;
            }
            while (rawRes.Contains(add) || tries < gr1.Count * 2);
            rawRes.Add(add);
        }
        return rawRes.ToArray();
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
