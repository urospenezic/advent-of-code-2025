using System;

namespace MyScript;

/*
Current segment to evaluate: 1322232245222415222432632212222232132125222522222424251221232212222224137321221142152242222112122421
Current max digits: 7, remaining segment: 321221142152242222112122421
Current segment to evaluate: 321221142152242222112122421
Current max digits: 75, remaining segment: 2242222112122421
Current segment to evaluate: 2242222112122421
Current max digits: 754, remaining segment: 2222112122421
Current segment to evaluate: 2222112122421
Current max digits: 7542, remaining segment: 222112122421
Current segment to evaluate: 222112122421
Current max digits: 75422, remaining segment: 22112122421
Current segment to evaluate: 22112122421
Current max digits: 754222, remaining segment: 2112122421
Current segment to evaluate: 2112122421
Current max digits: 7542222, remaining segment: 112122421
Current segment to evaluate: 112122421
Current max digits: 75422222, remaining segment: 122421
Current segment to evaluate: 122421
Current max digits: 754222222, remaining segment: 2421
Current segment to evaluate: 2421
Current max digits: 7542222224, remaining segment: 21
Final max digits for bank segment: 754222222421, attempting parse to long.
*/
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
