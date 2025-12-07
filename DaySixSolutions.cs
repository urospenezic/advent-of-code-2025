using System;
using System.Data;
using System.Text.RegularExpressions;

namespace MyScript;

public static partial class DaySixSolutions
{
    private const string regex = @" *\d+";

    public static long SolvePartOne(string input)
    {
        var parsed = ParseInput(input);

        long result = 0;
        foreach (var (values, operation) in parsed)
        {
            if (operation)
            {
                result += values.Aggregate((long)1, (a, b) => a * b);
            }
            else
            {
                result += values.Aggregate((long)0, (a, b) => a + b);
            }
        }
        Console.WriteLine($"Final Result: {result}");
        return result;
    }

    public static long SolvePartTwo(string input)
    {
        var parsed = CephalopodsParse(input);

        long result = 0;
        foreach (var (values, operation) in parsed)
        {
            if (operation)
            {
                result += values.Aggregate((long)1, (a, b) => a * b);
            }
            else
            {
                result += values.Aggregate((long)0, (a, b) => a + b);
            }
        }
        Console.WriteLine($"Final Result: {result}");
        return result;
    }

    //hacky but i cannot deal with grid rotations, transposes and splits atm
    private static List<(List<int> values, bool operation)> CephalopodsParse(string input)
    {
        var rows = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var lastRow = rows[^1];
        var operators = lastRow.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        List<(List<int> values, bool operation)> problems = [];
        for (int i = 0; i < operators.Length; i++)
        {
            problems.Add(new() { operation = operators[i] == "*", values = [] });
        }

        var problemIndex = 0;
        for (var index = 0; index < lastRow.Length; index++)
        {
            var nextOpIndex = lastRow.IndexOfAny(['+', '*'], index + 1);
            if (nextOpIndex == -1)
            {
                nextOpIndex = lastRow.Length + 1;
            }
            var digitsCount = nextOpIndex - index - 1;
            for (var digitIndex = 0; digitIndex < digitsCount; digitIndex++)
            {
                var number = 0;
                for (var lineIndex = 0; lineIndex < rows.Length - 1; lineIndex++)
                {
                    var c = rows[lineIndex][index + digitIndex];
                    if (c != ' ')
                        number = number * 10 + (c - '0');
                }

                problems[problemIndex].values.Add(number);
            }

            index = nextOpIndex - 1;
            problemIndex++;
        }

        return problems;
    }

    private static List<(ICollection<int> values, bool operation)> ParseInput(string input)
    {
        var rows = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        List<(ICollection<int> values, bool operation)> problems = [];
        var operators = rows[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < operators.Length; i++)
        {
            problems.Add(new() { operation = operators[i] == "*", values = [] });
        }

        for (int i = 0; i < rows.Length - 1; i++)
        {
            var values = rows[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < values.Length; j++)
            {
                problems[j].values.Add(int.Parse(values[j]));
            }
        }
        return problems;
    }

    [GeneratedRegex(regex)]
    private static partial Regex MyRegex();
}
