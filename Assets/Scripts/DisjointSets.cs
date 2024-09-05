using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;

/* https://habr.com/ru/articles/647189/ */

public class Set
{
    public List<dgen2d.Edge> SetGraph;
    public List<Vertex> Vertices;

    public Set(dgen2d.Edge edge)
    {
        SetGraph = new(18) { edge };

        Vertices = [edge.P1, edge.P2];
    }

    public void Union(Set set, dgen2d.Edge connectingEdge)
    {
        SetGraph.AddRange(set.SetGraph);
        Vertices.AddRange(set.Vertices);
        SetGraph.Add(connectingEdge);
    }

    public void AddEdge(dgen2d.Edge edge)
    {
        SetGraph.Add(edge);
        Vertices.Add(edge.P1);
        Vertices.Add(edge.P2);
    }

    public bool Contains(Vertex vertex)
    {
        return Vertices.Contains(vertex);
    }
}

class SystemOfDisjointSets
{
    public List<Set> Sets;

    public SystemOfDisjointSets()
    {
        Sets = new List<Set>();
    }

    public void AddEdgeInSet(dgen2d.Edge edge)
    {
        Set setA = Find(edge.P1);
        Set setB = Find(edge.P2);

        if (setA != null && setB == null)
        {
            setA.AddEdge(edge);
        }
        else if (setA == null && setB != null)
        {
            setB.AddEdge(edge);
        }
        else if (setA == null && setB == null)
        {
            Set set = new Set(edge);
            Sets.Add(set);
        }
        else if (setA != null && setB != null)
        {
            if (setA != setB)
            {
                setA.Union(setB, edge);
                Sets.Remove(setB);
            }
        }
    }

    public Set Find(Vertex vertex)
    {
        foreach (Set set in Sets)
        {
            if (set.Contains(vertex)) return set;
        }
        return null;
    }
}