using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;

class Experiments
{
    public static void Experiment_1()
    {
        List<int> sizes = new List<int>() { 10, 20, 50, 100, 200, 500 };
        Stopwatch stopwatch = new Stopwatch();
        List<float> mean_results = new List<float>();
        
        Console.WriteLine("Rozpoczynam eksperymenty");
        Console.Write("Proszę czekać");

        foreach (int h in sizes)
        {
            Console.Write(".");
            stopwatch.Start();
            for (int k = 0; k < 20; k++)
            {
                string inputPath = "input.txt";
                InputGenerator.Generate(inputPath, n: h);

                var lines = File.ReadAllLines(inputPath);
                int n = lines.Length;

                var L = Enumerable.Range(0, n).Select(i => new Vertex(i.ToString())).ToList();
                var R = Enumerable.Range(n, n).Select(i => new Vertex(i.ToString())).ToList();
                var edges = new List<Models.Edge>();

                for (int i = 0; i < n; i++)
                {
                    var tokens = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != n)
                    {
                        throw new Exception("Bivariate classes have different counts");
                    }
                    for (int j = 0; j < n; j++)
                    {
                        string token = tokens[j].Trim().ToLower();
                        if (token != "n")
                        {
                            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double weight))
                            {
                                edges.Add(new Models.Edge(L[i], R[j], weight));
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

                foreach (var l in L)
                {
                    if (!connectedVertices.Contains(l))
                    {
                        return;
                    }
                }
                foreach (var r in R)
                {
                    if (!connectedVertices.Contains(r))
                    {
                        return;
                    }
                }

                // 1. Get the maximum weight
                double maxWeight = edges.Max(edge => edge.Weight);

                // 2. Create the new list with transformed weights
                var edgesChanged = edges.Select(edge => new Models.Edge(edge.Left, edge.Right, maxWeight - edge.Weight))
                                        .ToList();

                var hungarian = new HungarianAlgorithm(L, R, edgesChanged);
                var matching = hungarian.Run();
            }
            stopwatch.Stop();
            mean_results.Add(stopwatch.ElapsedMilliseconds / 20);
        }

        // Convert List<int> to double[] for ScottPlot's X-axis
        double[] xData = sizes.Select(s => (double)s).ToArray();
        // Convert List<float> to double[] for ScottPlot's Y-axis
        double[] yData = mean_results.Select(r => (double)r).ToArray();

        // Generate y-data for the N^3 complexity line
        // You'll need to find a suitable scaling factor (e.g., 0.000001)
        // to make the n^3 curve comparable to your mean_results.
        // Start with 1.0 and adjust it until the curve roughly matches your data's scale.
        double scaleFactor = 0.000001; // Adjust this value!
        double[] nCubedData = xData.Select(n => Math.Pow(n, 3) * scaleFactor).ToArray();


        var plt = new Plot(); // Create a new Plot object

        // Add scatter plot for actual results (dots only)
        var scatterPlot = plt.Add.Scatter(xData, yData);
        scatterPlot.Color = ScottPlot.Colors.Blue;
        scatterPlot.LegendText = "Mean Execution Time (Actual)";
        scatterPlot.MarkerSize = 5;
        scatterPlot.LineStyle.IsVisible = false; // Hide the line for the scatter plot

        // Add a line plot for actual results (connecting points, no markers)
        var trendLine = plt.Add.Scatter(xData, yData);
        trendLine.Color = ScottPlot.Colors.Red;
        trendLine.LegendText = "Mean Execution Time (Trend)";
        trendLine.LineStyle.IsVisible = true;
        trendLine.MarkerStyle.IsVisible = false;
        trendLine.LineStyle.Width = 2;

        // Add the N^3 complexity line
        var nCubedPlot = plt.Add.Scatter(xData, nCubedData);
        nCubedPlot.Color = ScottPlot.Colors.Green;
        nCubedPlot.LegendText = $"O(n^3) (scaled by {scaleFactor})"; // Indicate the scaling factor
        nCubedPlot.LineStyle.IsVisible = true;
        nCubedPlot.MarkerStyle.IsVisible = false;
        nCubedPlot.LineStyle.Width = 2;
        nCubedPlot.LineStyle.Pattern = LinePattern.Dashed; // Make it dashed for distinction


        // Customize the plot
        plt.Title("Hungarian Algorithm Performance vs. Input Size");
        plt.XLabel("Input Size (n)");
        plt.YLabel("Time (ms)"); // Changed to "Time (ms)" as it now includes a theoretical curve
        plt.Legend.IsVisible = true;
        plt.Legend.Alignment = ScottPlot.Alignment.UpperLeft;

        // Set axis limits (optional, but good for consistent scaling)
        // Ensure Y-axis limits accommodate both actual data and the scaled N^3 data
        double maxY = Math.Max(yData.Max(), nCubedData.Max());
        plt.Axes.SetLimitsX(0, sizes.Max() + 20);
        plt.Axes.SetLimitsY(0, maxY * 1.1);

        // Save the plot to an image file
        string plotFileName = "performance_plot_with_n3.png"; // Changed filename to reflect new plot
        plt.SavePng(plotFileName, 1000, 600);

        Console.WriteLine($"\nWykres zapisany do {plotFileName}");
    }

}

