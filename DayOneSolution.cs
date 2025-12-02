using System;

namespace MyScript;

public static class DayOneSolution
{
    public static int SolvePartOne(string input)
    {
        var start = 50;
        int count = 0;

        foreach (var line in input.Trim().Split("\n"))
        {
            var value = int.Parse(line.Substring(1));
            if (line.StartsWith("L", StringComparison.OrdinalIgnoreCase))
            {
                start = (start - value + 100) % 100;
            }
            else if (line.StartsWith("R", StringComparison.OrdinalIgnoreCase))
            {
                start = (start + value) % 100;
            }

            if (start == 0)
            {
                count++;
            }
        }

        Console.WriteLine($"Final Count: {count}");
        return count;
    }

    public static int SolvePartTwo(string input)
    {
        var start = 50;

        var count = 0;
        foreach (var line in input.Trim().Split("\n"))
        {
            var value = int.Parse(line.Substring(1));
            var fullSpins = value / 100;
            count += fullSpins;
            value %= 100;
            if (line.StartsWith("L", StringComparison.OrdinalIgnoreCase))
            {
                if (value >= start && start != 0)
                {
                    count++;
                }
                start = (start - value + 100) % 100;
            }
            else if (line.StartsWith("R", StringComparison.OrdinalIgnoreCase))
            {
                if (start + value >= 100 && start != 0)
                {
                    count++;
                }
                start = (start + value) % 100;
            }
        }

        Console.WriteLine($"Final Count: {count}");
        return count;
    }
}
