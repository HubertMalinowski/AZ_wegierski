namespace Models
{
    public class HungarianAlgorithm
    {
        private List<Vertex> L;
        private List<Vertex> R;
        private List<Edge> Edges;

        public HungarianAlgorithm(List<Vertex> left, List<Vertex> right, List<Edge> edges)
        {
            L = left;
            R = right;
            Edges = edges;
        }

        public List<(Vertex, Vertex)> Run()
        {
            // 1. Inicjalizacja etykiet
            foreach (var l in L)
            {
                var maxWeight = Edges.Where(e => e.Left == l).Max(e => e.Weight);
                l.Label = maxWeight;
            }

            foreach (var r in R)
                r.Label = 0;

            // 3. Inicjalizacja skojarzenia (puste)
            var matching = new List<(Vertex, Vertex)>();

            while (matching.Count < L.Count)
            {
                var path = FindAugmentingPath(matching);
                if (path == null || path.Count == 0)
                    throw new Exception("Brak ścieżki powiększającej.");

                // 5(b) - aktualizacja skojarzenia M ⊕ P
                foreach (var (u, v) in path)
                {
                    var existing = matching.FirstOrDefault(pair => pair.Item1 == v && pair.Item2 == u);
                    if (matching.Contains(existing))
                        matching.Remove(existing);
                    else
                        matching.Add((u, v));

                    u.MatchedWith = v;
                    v.MatchedWith = u;
                }
            }

            return matching;
        }

        private List<(Vertex, Vertex)> FindAugmentingPath(List<(Vertex, Vertex)> matching)
        {
            var queue = new Queue<Vertex>();
            var FL = new HashSet<Vertex>();
            var FR = new HashSet<Vertex>();

            foreach (var l in L.Where(l => l.IsFree))
            {
                l.Predecessor = null;
                queue.Enqueue(l);
                FL.Add(l);
            }

            Dictionary<Vertex, Vertex> pred = new Dictionary<Vertex, Vertex>();
            bool pathFound = false;
            Vertex? end = null;

            while (!pathFound)
            {
                if (queue.Count == 0)
                {
                    // Brak ścieżki - aktualizacja etykiet
                    double delta = double.PositiveInfinity;
                    foreach (var l in FL)
                    {
                        foreach (var r in R.Except(FR))
                        {
                            var w = Edges.FirstOrDefault(e => e.Left == l && e.Right == r)?.Weight ?? double.NegativeInfinity;
                            delta = Math.Min(delta, l.Label + r.Label - w);
                        }
                    }

                    foreach (var l in FL) l.Label -= delta;
                    foreach (var r in FR) r.Label += delta;
                    continue;
                }

                var u = queue.Dequeue();

                foreach (var edge in Edges.Where(e => e.Left == u && Math.Abs(e.Left.Label + e.Right.Label - e.Weight) < 1e-9))
                {
                    var v = edge.Right;
                    if (FR.Contains(v)) continue;

                    v.Predecessor = u;
                    FR.Add(v);

                    if (v.IsFree)
                    {
                        end = v;
                        pathFound = true;
                        break;
                    }
                    else
                    {
                        var next = v.MatchedWith;
                        if (next != null &&!FL.Contains(next))
                        {
                            next.Predecessor = v;
                            queue.Enqueue(next);
                            FL.Add(next);
                        }
                    }
                }
            }

            // Odtworzenie ścieżki powiększającej
            var path = new List<(Vertex, Vertex)>();
            while (end?.Predecessor != null)
            {
                var u = end.Predecessor;
                path.Add((u, end));
                end = u.Predecessor;
            }

            path.Reverse();
            return path;
        }
    }
}
