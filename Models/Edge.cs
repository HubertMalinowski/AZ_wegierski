namespace Models
{
    public class Edge
    {
        public Vertex Left;
        public Vertex Right;
        public double Weight;

        public Edge(Vertex l, Vertex r, double w)
        {
            Left = l;
            Right = r;
            Weight = w;
        }
    }
}
