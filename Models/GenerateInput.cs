using System;
using System.IO;

public class InputGenerator
{
    public static void Generate(string filePath, int n, double missingProbability = 0.3, int minWeight = 1, int maxWeight = 10)
    {
        var rand = new Random();

        using var writer = new StreamWriter(filePath);
        for (int i = 0; i < n; i++)
        {
            string[] row = new string[n];
            for (int j = 0; j < n; j++)
            {
                if (rand.NextDouble() < missingProbability)
                    row[j] = "n";
                else
                    row[j] = rand.Next(minWeight, maxWeight + 1).ToString();
            }
            writer.WriteLine(string.Join(' ', row));
        }

        Console.WriteLine($"Wygenerowano plik wejściowy: {filePath}");
    }
}
