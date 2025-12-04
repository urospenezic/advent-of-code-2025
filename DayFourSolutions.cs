using System;
using System.Reflection.PortableExecutable;
using Microsoft.VisualBasic;

namespace MyScript;

public class DayFourSolutions
{
    const char BoxChar = '@';
    const char EmptyChar = '.';

    public static int SolvePartTwo(string input, int threshold)
    {
        var grid = ParseInput(input);
        int validBoxCount = 0;
        while (true)
        {
            (var validBoxes, var indexesToRemove) = SolveFor(threshold, grid);
            if (validBoxes == 0)
            {
                break;
            }
            foreach (var (row, column) in indexesToRemove)
            {
                grid[row][column] = EmptyChar;
            }
            validBoxCount += validBoxes;
        }

        Console.WriteLine($"Valid boxes found: {validBoxCount}");
        return validBoxCount;
    }

    public static int SolvePartOne(string input, int threshold)
    {
        var grid = ParseInput(input);
        (var validBoxCount, var indexesToRemove) = SolveFor(threshold, grid);
        Console.WriteLine($"Valid boxes found: {validBoxCount}");
        return validBoxCount;
    }

    private static (int count, List<(int row, int column)> indexesToRemove) SolveFor(
        int threshold,
        char[][] grid
    )
    {
        var validBoxes = 0;
        var indexesToRemove = new List<(int row, int column)>();
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] != BoxChar)
                {
                    continue;
                }
                var sumOfBoxes = CountBoxesAround(grid, i, j);
                if (sumOfBoxes < threshold)
                {
                    validBoxes++;
                    indexesToRemove.Add((i, j));
                }
            }
        }

        return (validBoxes, indexesToRemove);
    }

    private static int CountBoxesAround(char[][] grid, int row, int col, int depth = 1)
    {
        int count = 0;
        (int rowStartBound, int rowEndBound) = (
            Math.Max(0, row - depth),
            Math.Min(grid.Length - 1, row + depth)
        );
        (int colStartBound, int colEndBound) = (
            Math.Max(0, col - depth),
            Math.Min(grid[0].Length - 1, col + depth)
        );
        for (int i = rowStartBound; i <= rowEndBound; i++)
        {
            for (int j = colStartBound; j <= colEndBound; j++)
            {
                if (i == row && j == col)
                {
                    continue;
                }
                if (grid[i][j] == BoxChar)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private static char[][] ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var result = new char[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            result[i] = lines[i].ToCharArray();
        }
        return result;
    }
}
