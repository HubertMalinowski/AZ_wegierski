namespace Models
{
    public class Vertex
    {
        public string Id;
        public double Label;
        public Vertex? MatchedWith { get; set; }
        public Vertex? Predecessor { get; set; }


        public Vertex(string id)
        {
            Id = id;
            Label = 0;
            MatchedWith = null;
            Predecessor = null;
        }

        public bool IsFree => MatchedWith == null;
    }
}
