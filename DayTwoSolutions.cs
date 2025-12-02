using System;

namespace MyScript;

public static class ValidityChecks
{
    public static bool IsValidNumberPartOne(string number)
    {
        if (number.Length <= 1 || number.Length % 2 != 0)
        {
            return true;
        }

        //set pointer at halfway point and compare digits from start to middle and middle to end
        var halfwayIndex = number.Length > 2 ? number.Length / 2 : 1;
        var midPointer = halfwayIndex;
        // 3 4 5 6 7 8
        // 1 2 3 1 2 3 1 2 3 1 2 3
        for (int i = 0; i < halfwayIndex; i++)
        {
            var left = int.Parse(number[i].ToString());
            var right = int.Parse(number[midPointer].ToString());
            midPointer++;

            if (left != right)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsValidNumberPartTwo(string number)
    {
        if (number.Length <= 1)
        {
            return true;
        }
        //22
        // 1 2 3 1 2 3 1 2 3 1 2 3
        var halfway = number.Length > 2 ? number.Length / 2 : 1;
        int sampleSize = 1;
        while (sampleSize <= halfway)
        {
            if (!IsValidNumberSegment(number, sampleSize))
            {
                return false;
            }
            sampleSize++;
        }
        return true;
    }

    private static bool IsValidNumberSegment(string number, int sampleSize)
    {
        var sample = number[..sampleSize];
        var timesToRepeat = number.Length / sampleSize;
        var repeatedSample = string.Concat(Enumerable.Repeat(sample, timesToRepeat));
        if (repeatedSample.Equals(number))
        {
            Console.WriteLine($"Invalid segment found: {sample} repeated {timesToRepeat} times.");
            return false;
        }
        return true;
    }
}

public static class DayTwoSolutions
{
    public static long Solve(string input, Func<string, bool> isValidNumberFunc)
    {
        var ranges = input.Trim().Split(",");
        var invalidIndexes = new List<long>();
        //cache min start and max end as range to optimize checking

        foreach (var range in ranges)
        {
            var (start, end) = ParseRange(range);

            for (long i = start; i <= end; i++)
            {
                var numberStr = i.ToString();
                if (!isValidNumberFunc(numberStr))
                {
                    Console.WriteLine($"Invalid Number Found: {i}");
                    invalidIndexes.Add(i);
                }
            }
        }

        long sum = 0;
        foreach (var invalid in invalidIndexes)
        {
            sum += invalid;
        }

        Console.WriteLine($"Final Sum: {sum}");
        return sum;
    }

    private static (long start, long end) ParseRange(string range)
    {
        var parts = range.Split("-");
        var start = long.Parse(parts[0]);
        var end = long.Parse(parts[1]);
        return (start, end);
    }
}
