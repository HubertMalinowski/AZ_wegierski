using Models;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        // Do zakomentowania, aby samemu podać plik wejściowy
        InputGenerator.Generate("input.txt", n: 3);

        string inputPath = "input.txt";
        string outputPath = "output.txt";

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

        var hungarian = new HungarianAlgorithm(L, R, edges);
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
