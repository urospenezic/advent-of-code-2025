using System;

namespace MyScript;

public static class DayThreeSolutions
{
    public static long Solve(string input, int bankSize = 12)
    {
        var banks = input.Split('\n');

        long sum = 0;
        foreach (var bank in banks)
        {
            if (string.IsNullOrWhiteSpace(bank))
            {
                continue;
            }
            if (bank.Length < bankSize)
            {
                continue;
            }
            sum += FindMaxJoltage(bank, bankSize);
        }

        Console.WriteLine($"Day three solution: {sum}");
        return sum;
    }

    public static long FindMaxJoltage(string bankSegment, int bankSize)
    {
        string maxDigits = string.Empty;
        string currentSegment = bankSegment;
        while (maxDigits.Length + currentSegment.Length > bankSize && maxDigits.Length < bankSize)
        {
            var max = -1;
            var substringIndex = 0;
            for (int i = 0; i < currentSegment.Length - (bankSize - maxDigits.Length) + 1; i++)
            {
                var joltage = int.Parse(currentSegment[i].ToString());
                if (joltage > max)
                {
                    max = joltage;
                    substringIndex = i;
                }
            }
            maxDigits = $"{maxDigits}{max}";
            currentSegment = currentSegment.Substring(substringIndex + 1);
        }

        if (maxDigits.Length < bankSize && currentSegment.Length > 0)
        {
            foreach (var ch in currentSegment)
            {
                maxDigits = $"{maxDigits}{ch}";
                if (maxDigits.Length >= bankSize)
                {
                    break;
                }
            }
        }

        return long.Parse(maxDigits);
    }
}
