using System;
using System.Drawing;

namespace MyScript;

public class DayNineSolutions
{
    public static long SolvePartOne(string input, Point[]? cachedPoints = null)
    {
        var points = cachedPoints ?? ParseInput(input);
        (long maxArea, (int i, int j) point1, (int i, int j) point2) maxArea = (0, (0, 0), (0, 0));
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                long xDelta = Math.Abs(points[j].X - points[i].X) + 1;
                long yDelta = Math.Abs(points[j].Y - points[i].Y) + 1;
                long area = xDelta * yDelta;
                if (area > maxArea.maxArea)
                {
                    maxArea = (area, (i, j), (j, i));
                }
            }
        }
        // Implementation for Day Nine goes here
        Console.WriteLine($"Part One Result: {maxArea.maxArea}");
        return maxArea.maxArea;
    }

    public static long SolvePartTwo(string input)
    {
        /*
            summary of what this does:
            - parses input into points
            - creates edges for each side of the polygon formed by the points
            - iterates through all pairs of points to form rectangles
            - calculates area of each rectangle
            - projects all 4 edges of the rectangle we're about to test
            - checks if rectangle is valid (does not intersect polygon edges - geometric check and is inside polygon - ray casting algorithm)
            - keeps track of maximum valid rectangle area found
            - returns maximum area found
        */
        var points = ParseInput(input);
        int n = points.Length;
        var polygonEdges = new Edge[n];
        for (int i = 0; i < n; i++)
        {
            Point a = points[i];
            Point b = points[(i + 1) % n];
            polygonEdges[i] = new Edge(a.X, a.Y, b.X, b.Y);
        }

        long maxArea = 0;

        for (int i = 0; i < n; i++)
        {
            var p1 = points[i];

            for (int j = i + 1; j < n; j++)
            {
                var p2 = points[j];

                long xDelta = Math.Abs(p2.X - p1.X) + 1;
                long yDelta = Math.Abs(p2.Y - p1.Y) + 1;
                long area = xDelta * yDelta;
                //no reason to test if valid
                if (area <= maxArea)
                {
                    continue;
                }

                if (IsRectangleOutsideOfBounds(p1, p2, polygonEdges, points))
                {
                    continue;
                }

                maxArea = area;
            }
        }

        Console.WriteLine($"Part Two Result: {maxArea}");
        return maxArea;
    }

    private readonly struct Edge
    {
        public bool IsHorizontal { get; }
        public double X1 { get; }
        public double Y1 { get; }
        public double X2 { get; }
        public double Y2 { get; }

        public Edge(double x1, double y1, double x2, double y2)
        {
            if (Math.Abs(y1 - y2) < 1e-9)
            {
                IsHorizontal = true;
                if (x1 <= x2)
                {
                    X1 = x1;
                    Y1 = y1;
                    X2 = x2;
                    Y2 = y2;
                }
                else
                {
                    X1 = x2;
                    Y1 = y2;
                    X2 = x1;
                    Y2 = y1;
                }
            }
            else
            {
                IsHorizontal = false;
                if (y1 <= y2)
                {
                    X1 = x1;
                    Y1 = y1;
                    X2 = x2;
                    Y2 = y2;
                }
                else
                {
                    X1 = x2;
                    Y1 = y2;
                    X2 = x1;
                    Y2 = y1;
                }
            }
        }
    }

    private static bool SegmentsIntersect(in Edge a, in Edge b)
    {
        //parallel lines can't intersect
        if (a.IsHorizontal == b.IsHorizontal)
        {
            return false;
        }

        var h = a.IsHorizontal ? a : b;
        var v = a.IsHorizontal ? b : a;

        //touching at endpoints doesn't count as intersecting
        return v.X1 > h.X1 && v.X1 < h.X2 && h.Y1 > v.Y1 && h.Y1 < v.Y2;
    }

    private static bool IsRectangleOutsideOfBounds(
        Point p1,
        Point p2,
        Edge[] polygonEdges,
        Point[] polygonPoints
    )
    {
        double minX = Math.Min(p1.X, p2.X) + 0.5;
        double maxX = Math.Max(p1.X, p2.X) - 0.5;
        double minY = Math.Min(p1.Y, p2.Y) + 0.5;
        double maxY = Math.Max(p1.Y, p2.Y) - 0.5;
        //no area basically
        if (minX > maxX || minY > maxY)
        {
            return true;
        }

        //check if any of the rectangle's edges intersect with any of the polygon's edges
        Span<Edge> rectEdges =
        [
            new Edge(minX, minY, maxX, minY), //top
            new Edge(maxX, minY, maxX, maxY), //right
            new Edge(maxX, maxY, minX, maxY), //bottom
            new Edge(minX, maxY, minX, minY), //left
        ];
        foreach (var rectEdge in rectEdges)
        {
            for (int i = 0; i < polygonEdges.Length; i++)
            {
                if (SegmentsIntersect(rectEdge, polygonEdges[i]))
                {
                    return true;
                }
            }
        }

        double centerX = (minX + maxX) * 0.5;
        double centerY = (minY + maxY) * 0.5;

        if (!IsInsidePolygon(centerX, centerY, polygonPoints))
        {
            return true;
        }

        return false;
    }

    private static bool IsInsidePolygon(double x, double y, Point[] polygon)
    {
        bool inside = false;
        int n = polygon.Length;

        for (int i = 0, j = n - 1; i < n; j = i, i++)
        {
            var pi = polygon[i];
            var pj = polygon[j];

            bool intersects =
                (pi.Y > y) != (pj.Y > y)
                && x < (pj.X - pi.X) * (y - pi.Y) / (double)(pj.Y - pi.Y) + pi.X;

            if (intersects)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    private static Point[] ParseInput(string input)
    {
        var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Point[] points = new Point[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            points[i] = new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
        return points;
    }
}
