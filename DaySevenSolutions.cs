using System;

namespace MyScript;

public class DaySevenSolutions
{
    private const char BeamChar = '|';
    private const char StationChar = 'S';
    private const char SplitterChar = '^';

    /*
.......S.......
.......|.......
......|^|......
......|.|......
.....|^|^|.....
.....|.|.|.....
....|^|^|^|....
....|.|.|.|....
...|^|^|||^|...
...|.|.|||.|...
..|^|^|||^|^|..
..|.|.|||.|.|..
.|^|||^||.||^|.
.|.|||.||.||.|.
|^|^|^|^|^|||^|
|.|.|.|.|.|||.|
    */
    public static int SolvePartOne(string input)
    {
        int result = 0;

        var grid = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();
        int starterBeamPosition = grid[0].IndexOf(StationChar);
        if (starterBeamPosition == -1)
            return 0;

        HashSet<int> beamPositions = [];
        beamPositions.Add(starterBeamPosition);
        int current = 1;
        while (current < grid.Length)
        {
            var row = grid[current];
            var splitterIndex = row.IndexOf(SplitterChar);
            if (splitterIndex == -1)
            {
                current++;
                continue;
            }
            HashSet<int> splitterPositions = [];
            splitterPositions.Add(splitterIndex);

            while (splitterIndex != -1 && splitterIndex < row.Length)
            {
                splitterIndex = Array.IndexOf(row, SplitterChar, splitterIndex + 1);
                if (splitterIndex == -1)
                    break;
                splitterPositions.Add(splitterIndex);
            }
            var intersections = beamPositions.Intersect(splitterPositions).ToList();
            result += intersections.Count;
            beamPositions.RemoveWhere(intersections.Contains);

            foreach (var pos in intersections)
            {
                if (pos - 1 >= 0)
                    beamPositions.Add(pos - 1);
                if (pos + 1 < row.Length)
                    beamPositions.Add(pos + 1);
            }
            current++;
        }

        Console.WriteLine($"Final Result: {result}");
        return result;
    }

    //this one will have to be done via dfs/inline tree traversal
    public static long SolvePartTwo(string input)
    {
        var grid = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();

        int rowCount = grid.Length;
        if (rowCount == 0)
            return 0;

        int colCount = grid[0].Length;

        int starterBeamPosition = grid[0].IndexOf(StationChar);
        if (starterBeamPosition == -1)
            return 0;

        var visitedPointPathsResult = new Dictionary<(int Row, int Col), long>();

        long Dfs(int row, int col)
        {
            if (row >= rowCount || col < 0 || col >= colCount)
                return 1;

            var key = (row, col);
            if (visitedPointPathsResult.TryGetValue(key, out var cached))
                return cached;

            char cell = grid[row][col];

            long result;
            if (cell == SplitterChar)
            {
                //project 2 potential paths
                result = Dfs(row + 1, col - 1) + Dfs(row + 1, col + 1);
            }
            else
            {
                //empty slot, skip to next row
                result = Dfs(row + 1, col);
            }

            visitedPointPathsResult[key] = result;
            return result;
        }

        long totalTimelines = Dfs(1, starterBeamPosition);
        Console.WriteLine($"Final Result Part Two: {totalTimelines}");
        return totalTimelines;
    }
}
