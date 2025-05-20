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
            // 1. Initialize labels
            foreach (var l in L)
            {
                var maxWeight = Edges.Where(e => e.Left == l).Max(e => e.Weight);
                l.Label = maxWeight;
            }
            foreach (var r in R)
                r.Label = 0;

            // 2. Initialize matching (empty)
            var matching = new List<(Vertex, Vertex)>();

            // Main loop: Continue until we have a perfect matching
            while (matching.Count < L.Count)
            {
                var path = FindAugmentingPath(matching); // Call FindAugmentingPath

                if (path == null) // No augmenting path found in the current equality graph
                {
                    // Update labels to change the equality graph and try again.
                    // This is the core of the Hungarian algorithm's iterative improvement.

                    // Calculate delta (minimum slack)
                    double delta = double.PositiveInfinity;
                    HashSet<Vertex> FL = new HashSet<Vertex>(); // Vertices in L visited
                    HashSet<Vertex> FR = new HashSet<Vertex>(); // Vertices in R visited

                    // We need to determine FL and FR to calculate delta.
                    // FindAugmentingPath could return these, but for now, recalculate.
                    Queue<Vertex> queue = new Queue<Vertex>();
                    foreach (var l in L.Where(l => l.IsFree)) // Start with free L vertices
                    {
                        queue.Enqueue(l);
                        FL.Add(l);
                    }

                    while (queue.Count > 0)
                    {
                        var u = queue.Dequeue();
                        if (L.Contains(u)) // u is in L
                        {
                            foreach (var edge in Edges.Where(e => e.Left == u && Math.Abs(e.Left.Label + e.Right.Label - e.Weight) < 1e-9))
                            {
                                var v = edge.Right;
                                if (!FR.Contains(v))
                                {
                                    FR.Add(v);
                                    if (v.MatchedWith != null) // v is matched
                                    {
                                        var nextL = v.MatchedWith;
                                        if (nextL != null && !FL.Contains(nextL))
                                        {
                                            FL.Add(nextL);
                                            queue.Enqueue(nextL);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var l in FL)
                    {
                        foreach (var r in R.Except(FR))
                        {
                            var w = Edges.FirstOrDefault(e => e.Left == l && e.Right == r)?.Weight ?? double.NegativeInfinity;
                            delta = Math.Min(delta, l.Label + r.Label - w);
                        }
                    }

                    // Update labels
                    foreach (var l in FL) l.Label -= delta;
                    foreach (var r in FR) r.Label += delta;

                    continue; // Go back to the top of the while loop
                }

                // If we get here, FindAugmentingPath found an augmenting path. Augment the matching.
                foreach (var (u, v) in path)
                {
                    var existing = matching.FirstOrDefault(pair => pair.Item1 == u && pair.Item2 == v);
                    if (existing != default)
                    {
                        matching.Remove(existing);
                    }
                    else
                    {
                        matching.Add((u, v));
                    }

                    u.MatchedWith = u.MatchedWith == v ? null : v;
                    v.MatchedWith = v.MatchedWith == u ? null : u;
                }

                List<(Vertex, Vertex)> toRemove = new List<(Vertex, Vertex)>();
                foreach(var (u, v) in matching)
                {
                    if(u.MatchedWith != v || v.MatchedWith != u)
                    {
                        toRemove.Add((u, v));
                    }
                }
                matching = matching.Where(v => !toRemove.Contains(v)).ToList();
            }

            return matching;
        }

        private List<(Vertex, Vertex)> FindAugmentingPath(List<(Vertex, Vertex)> matching)
        {
            // Reset search state at the beginning of each call!
            foreach (var l in L)
            {
                l.Predecessor = null;
            }
            foreach (var r in R)
            {
                r.Predecessor = null;
            }

            var queue = new Queue<Vertex>();
            var FL = new HashSet<Vertex>();
            var FR = new HashSet<Vertex>();

            // Initialize BFS from free vertices in L
            foreach (var l in L.Where(l => l.IsFree))
            {
                queue.Enqueue(l);
                FL.Add(l);
            }

            bool pathFound = false;
            Vertex? end = null;

            while (!pathFound)
            {
                if (queue.Count == 0)
                {
                    return null; // Signal to the Run() method that labels were updated, and no path was found.
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
                        if (next != null && !FL.Contains(next))
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
