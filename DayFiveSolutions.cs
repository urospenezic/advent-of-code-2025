using System;

namespace MyScript;

public static class DayFiveSolutions
{
    public static long SolvePartOne(string input)
    {
        var (ranges, numbers) = ParseInput(input);
        var mergedRanges = MergeRanges(ranges);
        long valid = 0;

        foreach (var number in numbers)
        {
            if (IsInAnyRange(number, mergedRanges))
            {
                valid++;
            }
        }

        Console.WriteLine($"Sum of valid numbers: {valid}");
        return valid;
    }

    public static long SolvePartTwo(string input)
    {
        var (ranges, numbers) = ParseInput(input);
        var mergedRanges = MergeRanges(ranges);
        long countAllNumbersInRanges = mergedRanges.Select(r => r.high - r.low + 1).Sum();

        Console.WriteLine($"Number of valid ingreadiants: {countAllNumbersInRanges}");
        return countAllNumbersInRanges;
    }

    private static List<(long low, long high)> MergeRanges(List<(long low, long high)> ranges)
    {
        if (ranges.Count == 0)
        {
            return new List<(long low, long high)>();
        }

        var sortedRanges = ranges.OrderBy(r => r.low).ThenBy(r => r.high).ToList();
        var mergedRanges = new List<(long low, long high)>();
        var currentRange = sortedRanges[0];

        for (int i = 1; i < sortedRanges.Count; i++)
        {
            var nextRange = sortedRanges[i];
            if (nextRange.low <= currentRange.high + 1)
            {
                currentRange.high = Math.Max(currentRange.high, nextRange.high);
            }
            else
            {
                mergedRanges.Add(currentRange);
                currentRange = nextRange;
            }
        }
        mergedRanges.Add(currentRange);

        return mergedRanges;
    }

    private static bool IsInAnyRange(long number, List<(long low, long high)> merged)
    {
        int lo = 0,
            hi = merged.Count - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            var r = merged[mid];
            if (number < r.low)
                hi = mid - 1;
            else if (number > r.high)
                lo = mid + 1;
            else
                return true;
        }
        return false;
    }

    private static (List<(long low, long high)> ranges, List<long> numbers) ParseInput(string input)
    {
        var lines = input.Split(
            Environment.NewLine + Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries
        );

        if (lines.Length != 2)
        {
            throw new ArgumentException(
                "Input must contain exactly two sections separated by a blank line."
            );
        }

        var ranges = lines[0].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var numbers = lines[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var parsedRanges = new List<(long low, long high)>();
        foreach (var range in ranges)
        {
            var parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (
                parts.Length != 2
                || !long.TryParse(parts[0], out long low)
                || !long.TryParse(parts[1], out long high)
            )
            {
                throw new ArgumentException($"Invalid range format: {range}");
            }
            parsedRanges.Add((low, high));
        }

        var parsedNumbers = new List<long>();
        foreach (var number in numbers)
        {
            if (!long.TryParse(number, out long num))
            {
                throw new ArgumentException($"Invalid number format: {number}");
            }
            parsedNumbers.Add(num);
        }
        return (parsedRanges, parsedNumbers);
    }
}
