using System;
using Microsoft.VisualBasic;

namespace MyScript;

public class DayElevenSolutions
{
    private const string start = "you";
    private const string end = "out";

    public static int SolvePartOne(string input)
    {
        var graph = ParseInput(input);
        var cachedPaths = new Dictionary<(string from, string to), long>();
        return (int)Dfs(graph, start, end, cachedPaths);
    }

    public static long SolvePartTwo(string input)
    {
        var graph = ParseInput(input);
        //find which of fft/dac is closer to svr bfs
        var firstMidpoint = Bfs(graph, "svr", "fft", "dac");
        var secondMidpoint = firstMidpoint == "dac" ? "fft" : "dac";
        Console.WriteLine($"First midpoint: {firstMidpoint}, second midpoint: {secondMidpoint}");
        var cachedPaths = new Dictionary<(string from, string to), long>();

        long p1 = Dfs(graph, "svr", firstMidpoint, cachedPaths);
        Console.WriteLine($"Paths from svr to {firstMidpoint}: {p1}");
        long p2 = Dfs(graph, firstMidpoint, secondMidpoint, cachedPaths);
        Console.WriteLine($"Paths from {firstMidpoint} to {secondMidpoint}: {p2}");
        long p3 = Dfs(graph, secondMidpoint, end, cachedPaths);
        Console.WriteLine($"Paths from {secondMidpoint} to out: {p3}");
        return p1 * p2 * p3;
    }

    //assume no cycles
    private static long Dfs(
        Dictionary<string, string[]> graph,
        string from,
        string to,
        Dictionary<(string from, string to), long> cachedPaths
    )
    {
        if (from == to)
        {
            return 1;
        }

        if (from == end)
        {
            return 0;
        }

        var key = (from, to);
        if (cachedPaths.TryGetValue(key, out var cached))
        {
            return cached;
        }

        if (!graph.TryGetValue(from, out var children))
        {
            cachedPaths[key] = 0;
            return 0;
        }

        long paths = 0;
        foreach (var child in children)
        {
            paths += Dfs(graph, child, to, cachedPaths);
        }

        cachedPaths[key] = paths;
        return paths;
    }

    private static string Bfs(
        Dictionary<string, string[]> graph,
        string startNode,
        string a,
        string b
    )
    {
        var queue = new Queue<string>();
        var visited = new HashSet<string>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            var device = queue.Dequeue();

            if (device == a)
            {
                return a;
            }

            if (device == b)
            {
                return b;
            }

            if (!graph.TryGetValue(device, out var children))
            {
                continue;
            }

            foreach (var output in children)
            {
                if (visited.Add(output))
                {
                    queue.Enqueue(output);
                }
            }
        }

        throw new InvalidOperationException("Neither midpoint device is reachable from start.");
    }

    public static Dictionary<string, string[]> ParseInput(string input)
    {
        var result = new Dictionary<string, string[]>();
        var nodes = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var node in nodes)
        {
            var indexOfName = node.IndexOf(':');
            var name = node[..indexOfName].Trim();
            var children = node[(indexOfName + 1)..]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToArray();

            result[name] = children;
        }

        return result;
    }
}
