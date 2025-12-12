using System;
using System.Collections;
using System.Collections.Generic;

namespace MyScript;

public class DayTwelveSolutions
{
    public static long SolvePartOne(string input)
    {
        //solution is likely a backtracking algorithm to fit shapes into a grid
        //we recursively try to place each shape into the grid and backtrack if we reach a dead end
        //placing each shape involves rotating and flipping the shape to try and find a fit
        //caching/memoization of grid states and shapes already placed could help optimize performance
        //or to think of it we better just precomoute all possible placements for each shape type on this grid

        //the grid class would represent the current state of the grid and will be responsible for checking if a shape can be placed at a given position (and return a new grid state (new instance of Grid))
        //the shape class would represent the shape in various orientations (rotated/flipped) and provide methods to get those orientations
        //we can likely just keep the shapes as a matrix 0 and 1 representing empty and filled cells
        //each spot on the grid that has a 1 is occupied

        var parsed = ParseInput(input);

        long solvable = 0;
        foreach (var grid in parsed.Grids)
        {
            if (CanSolveGrid(parsed.Shapes, grid))
            {
                solvable++;
            }
        }

        return solvable;
    }

    public static long SolvePartTwo(string input)
    {
        // Implementation for Day Twelve Part Two
        throw new NotImplementedException();
    }

    private static ParsedInput ParseInput(string input)
    {
        var lines = input.Split([Environment.NewLine], StringSplitOptions.None);

        var shapes = new List<Shape>();
        int index = 0;

        while (index < lines.Length)
        {
            var line = lines[index].Trim();

            if (string.IsNullOrEmpty(line))
            {
                index++;
                continue;
            }
            //parse shape
            if (char.IsDigit(line[0]) && line.EndsWith(':'))
            {
                index++;
                if (index + 2 >= lines.Length)
                {
                    break;
                }

                bool[,] cells = new bool[3, 3];
                for (int y = 0; y < 3; y++)
                {
                    var row = lines[index++].Trim();
                    for (int x = 0; x < 3 && x < row.Length; x++)
                    {
                        cells[y, x] = row[x] == '#';
                    }
                }

                shapes.Add(new Shape(cells));
            }
            else
            {
                break;
            }
        }

        var grids = new List<Grid>();

        // parse grids
        for (; index < lines.Length; index++)
        {
            var line = lines[index].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            int colonIndex = line.IndexOf(':');
            if (colonIndex < 0)
            {
                continue;
            }

            var dims = line[..colonIndex].Trim();
            var rest = line[(colonIndex + 1)..].Trim();

            var dimParts = dims.Split('x');
            if (dimParts.Length != 2)
            {
                continue;
            }

            if (!int.TryParse(dimParts[0], out int width))
            {
                continue;
            }

            if (!int.TryParse(dimParts[1], out int height))
            {
                continue;
            }

            var countParts = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (countParts.Length != 6)
            {
                continue;
            }

            int[] counts = new int[6];
            bool ok = true;
            for (int i = 0; i < 6; i++)
            {
                if (!int.TryParse(countParts[i], out counts[i]))
                {
                    ok = false;
                    break;
                }
            }

            if (!ok)
            {
                continue;
            }

            grids.Add(new Grid(width, height, counts));
        }

        return new ParsedInput([.. shapes], [.. grids]);
    }

    private static bool CanSolveGrid(Shape[] shapes, Grid grid)
    {
        int totalCells = grid.Width * grid.Height;
        int totalShapeCells = 0;
        int totalShapes = 0;
        for (int i = 0; i < grid.ShapeCounts.Length; i++)
        {
            totalShapes += grid.ShapeCounts[i];
            totalShapeCells += grid.ShapeCounts[i] * 7; // each shape has 7 occupied cells
        }

        // simple area check
        if (totalShapeCells > totalCells)
        {
            return false;
        }

        if (totalShapes == 0)
        {
            return true;
        }

        int bitCount = totalCells;
        int ulongLength = (bitCount + 63) / 64;

        // precompute all possible placements for each shape type on this grid
        var placementsPerShape = new List<ulong[]>[6];
        for (int i = 0; i < 6; i++)
        {
            placementsPerShape[i] = new List<ulong[]>();
        }

        for (int type = 0; type < 6; type++)
        {
            if (grid.ShapeCounts[type] == 0)
            {
                continue;
            }

            var shape = shapes[type];
            foreach (var orientation in shape.Orientations)
            {
                int shapeWidth = orientation.Width;
                int shapeHeight = orientation.Height;

                // we require the full 3x3 footprint (after rotation/flip) to lie inside the grid
                if (shapeWidth != 3 || shapeHeight != 3)
                {
                    continue;
                }

                for (int y = 0; y <= grid.Height - shapeHeight; y++)
                {
                    for (int x = 0; x <= grid.Width - shapeWidth; x++)
                    {
                        ulong[] mask = new ulong[ulongLength];
                        bool valid = false;

                        for (int dy = 0; dy < shapeHeight; dy++)
                        {
                            for (int dx = 0; dx < shapeWidth; dx++)
                            {
                                if (!orientation.Cells[dy, dx])
                                {
                                    continue;
                                }

                                int cellIndex = (y + dy) * grid.Width + x + dx;
                                int arrayIndex = cellIndex / 64;
                                int bitIndex = cellIndex % 64;
                                mask[arrayIndex] |= 1UL << bitIndex;
                                valid = true;
                            }
                        }

                        if (valid)
                        {
                            placementsPerShape[type].Add(mask);
                        }
                    }
                }
            }
        }

        // if for some shape type we don't have enough distinct placements even ignoring overlap,
        // we can immediately fail.
        for (int i = 0; i < 6; i++)
        {
            if (grid.ShapeCounts[i] > 0 && placementsPerShape[i].Count < grid.ShapeCounts[i])
            {
                return false;
            }
        }

        ulong[] occupancy = new ulong[ulongLength];
        int[] remainingCounts = new int[6];
        int[] placementStart = new int[6];
        for (int i = 0; i < 6; i++)
        {
            remainingCounts[i] = grid.ShapeCounts[i];
            placementStart[i] = 0;
        }

        return Search(placementsPerShape, occupancy, remainingCounts, placementStart, totalShapes);
    }

    private static bool Search(
        List<ulong[]>[] placementsPerShape,
        ulong[] occupancy,
        int[] remainingCounts,
        int[] placementStart,
        int remainingTotal
    )
    {
        if (remainingTotal == 0)
        {
            return true;
        }

        // choose the next shape type to place: pick the one with remaining > 0 and
        // the fewest available placements
        int bestType = -1;
        int bestOptions = int.MaxValue;
        for (int i = 0; i < 6; i++)
        {
            if (remainingCounts[i] <= 0)
            {
                continue;
            }

            int options = placementsPerShape[i].Count - placementStart[i];
            if (options < bestOptions)
            {
                bestOptions = options;
                bestType = i;
            }
        }

        if (bestType == -1)
        {
            return false;
        }

        if (bestOptions <= 0)
        {
            return false;
        }

        var placementList = placementsPerShape[bestType];
        int startIndex = placementStart[bestType];

        for (int p = startIndex; p < placementList.Count; p++)
        {
            var mask = placementList[p];

            if (Intersects(occupancy, mask))
            {
                continue;
            }

            // place this shape instance
            ApplyMaskXor(occupancy, mask);
            remainingCounts[bestType]--;
            int oldStart = placementStart[bestType];
            placementStart[bestType] = p + 1;

            bool solved = Search(
                placementsPerShape,
                occupancy,
                remainingCounts,
                placementStart,
                remainingTotal - 1
            );

            if (solved)
            {
                return true;
            }

            // backtrack
            placementStart[bestType] = oldStart;
            remainingCounts[bestType]++;
            ApplyMaskXor(occupancy, mask);
        }

        return false;
    }

    private static bool Intersects(ulong[] occupancy, ulong[] mask)
    {
        for (int i = 0; i < occupancy.Length; i++)
        {
            if ((occupancy[i] & mask[i]) != 0)
            {
                return true;
            }
        }

        return false;
    }

    private static void ApplyMaskXor(ulong[] occupancy, ulong[] mask)
    {
        for (int i = 0; i < occupancy.Length; i++)
        {
            occupancy[i] ^= mask[i];
        }
    }

    private readonly record struct ParsedInput(Shape[] Shapes, Grid[] Grids);

    private readonly record struct Grid(int Width, int Height, int[] ShapeCounts);

    public class Shape(bool[,] baseCells)
    {
        public Orientation[] Orientations { get; } = BuildOrientations(baseCells);

        private static Orientation[] BuildOrientations(bool[,] baseCells)
        {
            var set = new Dictionary<string, Orientation>();

            void AddOrientation(bool[,] cells)
            {
                int h = cells.GetLength(0);
                int w = cells.GetLength(1);
                var keyChars = new char[h * w];
                int k = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        keyChars[k++] = cells[y, x] ? '1' : '0';
                    }
                }

                var key = new string(keyChars);
                if (!set.ContainsKey(key))
                {
                    set[key] = new Orientation(w, h, cells);
                }
            }

            bool[,] current = (bool[,])baseCells.Clone();

            for (int r = 0; r < 4; r++)
            {
                AddOrientation(current);

                var flipped = FlipHorizontal(current);
                AddOrientation(flipped);

                current = Rotate90(current);
            }

            var result = new Orientation[set.Count];
            int idx = 0;
            foreach (var orientation in set.Values)
            {
                result[idx++] = orientation;
            }

            return result;
        }

        private static bool[,] Rotate90(bool[,] cells)
        {
            int h = cells.GetLength(0);
            int w = cells.GetLength(1);
            var result = new bool[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // rotate clockwise around the 3x3 matrix
                    int newX = h - 1 - y;
                    int newY = x;
                    result[newY, newX] = cells[y, x];
                }
            }

            return result;
        }

        private static bool[,] FlipHorizontal(bool[,] cells)
        {
            int h = cells.GetLength(0);
            int w = cells.GetLength(1);
            var result = new bool[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int newX = w - 1 - x;
                    result[y, newX] = cells[y, x];
                }
            }

            return result;
        }

        public readonly record struct Orientation(int Width, int Height, bool[,] Cells);
    }
}
