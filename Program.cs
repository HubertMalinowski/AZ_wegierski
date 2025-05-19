using System;
using System.Collections.Generic;
using System.Linq;

using Models;
public class Program
{
    public static void Main(string[] args)
    {
        var L = new List<Vertex> { new Vertex("l1"), new Vertex("l2"), new Vertex("l3") };
        var R = new List<Vertex> { new Vertex("r1"), new Vertex("r2"), new Vertex("r3") };

        var edges = new List<Edge>
        {
            new Edge(L[0], R[0], 1),
            new Edge(L[0], R[1], 0),
            new Edge(L[0], R[2], 0),
            new Edge(L[1], R[0], 0),
            new Edge(L[1], R[1], 4),
            new Edge(L[1], R[2], 0),
            new Edge(L[2], R[0], 0),
            new Edge(L[2], R[1], 0),
            new Edge(L[2], R[2], 3)
        };

        var hungarian = new HungarianAlgorithm(L, R, edges);
        var matching = hungarian.Run();

        foreach (var (l, r) in matching)
            Console.WriteLine($"{l.Id} ↔ {r.Id}");
    }
}