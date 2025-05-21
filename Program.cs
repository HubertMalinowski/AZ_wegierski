using Models;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        string inputPath = "input.txt";
        string outputPath = "output.txt";
        using var writer = new StreamWriter(outputPath);

        // Ewentualne argumenty
        if (args.Length > 0)
        {
            if (args[0] == "--test")
            {
                Experiments.Experiment_1();
                return;
            }
            else if (args[0] == "--random")
            {
                if (args.Length > 1 && int.TryParse(args[1], out int randomN))
                {
                    inputPath = "input.txt";
                    outputPath = "output.txt";
                    InputGenerator.Generate(inputPath, randomN);
                    Console.WriteLine($"Wygenerowano plik wejściowy {inputPath}");
                }
                else
                {
                    Console.WriteLine("Nieprawidłowy argument. Użyj --random <n>, aby wygenerować losowe dane.");
                    inputPath = "input.txt";
                    outputPath = "output.txt";
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy argument. Użyj --test, aby uruchomić eksperymenty lub --random <n>, aby wygenerować losowe dane.");
                return;
            }
        }

        var lines = File.ReadAllLines(inputPath);
        int n = lines.Length;

        var L = Enumerable.Range(0, n).Select(i => new Vertex(i.ToString())).ToList();
        var R = Enumerable.Range(n, n).Select(i => new Vertex(i.ToString())).ToList();
        var edges = new List<Edge>();

        for (int i = 0; i < n; i++)
        {
            var tokens = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != n)
            {
                writer.WriteLine("Different size of bivarte classes");
                return;
            }
            for (int j = 0; j < n; j++)
            {
                string token = tokens[j].Trim().ToLower();
                if (token != "n")
                {
                    if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double weight))
                    {
                        edges.Add(new Edge(L[i], R[j], weight));
                    }
                    else
                    {
                        throw new Exception($"Nieprawidłowa wartość w pliku: {token}");
                    }
                }
                // brak krawędzi = brak dodania
            }
        }

        var connectedVertices = new HashSet<Vertex>();
        foreach (var edge in edges)
        {
            connectedVertices.Add(edge.Left);
            connectedVertices.Add(edge.Right);
        }

        foreach(var l in L)
        {
            if (!connectedVertices.Contains(l))
            {
                writer.WriteLine("Isolated vertex found, no matching");
                return;
            }
        }
        foreach (var r in R)
        {
            if (!connectedVertices.Contains(r))
            {
                writer.WriteLine("Isolated vertex found, no matching");
                return;
            }
        }

        // 1. Get the maximum weight
        double maxWeight = edges.Max(edge => edge.Weight);

        // 2. Create the new list with transformed weights
        var edgesChanged = edges.Select(edge => new Edge(edge.Left, edge.Right, maxWeight - edge.Weight))
                                .ToList();

        var hungarian = new HungarianAlgorithm(L, R, edgesChanged);
        var matching = hungarian.Run();

        // Oblicz łączną wagę
        double totalWeight = matching.Sum(pair =>
            edges.FirstOrDefault(e => e.Left == pair.Item1 && e.Right == pair.Item2)?.Weight ?? 0);

        writer.WriteLine(totalWeight.ToString(CultureInfo.InvariantCulture));

        foreach (var (l, r) in matching)
        {
            writer.WriteLine($"{l.Id} {r.Id}");
        }

        Console.WriteLine($"Zapisano wynik do pliku: {outputPath}");
    }
}
