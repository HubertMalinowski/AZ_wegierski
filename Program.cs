using Models;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {

        string inputPath = "C:\\Users\\Piotr\\source\\AZ\\AZ_wegierski\\input.txt";
        string outputPath = "C:\\Users\\Piotr\\source\\AZ\\AZ_wegierski\\output.txt";
        // Do zakomentowania, aby samemu podać plik wejściowy
        //InputGenerator.Generate(inputPath, n: 3);



        var lines = File.ReadAllLines(inputPath);
        int n = lines.Length;

        var L = Enumerable.Range(0, n).Select(i => new Vertex(i.ToString())).ToList();
        var R = Enumerable.Range(n, n).Select(i => new Vertex(i.ToString())).ToList();
        var edges = new List<Edge>();

        for (int i = 0; i < n; i++)
        {
            var tokens = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
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

        using var writer = new StreamWriter(outputPath);
        writer.WriteLine(totalWeight.ToString(CultureInfo.InvariantCulture));

        foreach (var (l, r) in matching)
        {
            writer.WriteLine($"{l.Id} {r.Id}");
        }

        Console.WriteLine($"Zapisano wynik do pliku: {outputPath}");
    }
}
