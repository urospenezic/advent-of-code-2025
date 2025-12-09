using System;

namespace MyScript;

public class DayEightSolutions
{
    //part 1 is just removing the if checks for one big conjuctions and limiting to 1000 distances
    public static long Solve(string input)
    {
        var points = ParseInput(input);

        List<(int indexPointOne, int indexPointTwo, long distance)> distances = [];
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                long dist = DistanceSquared(points[i], points[j]);
                distances.Add((i, j, dist));
            }
        }

        List<Conjuction> conjuctions = [];

        foreach (var (indexPointOne, indexPointTwo, _) in distances.OrderBy(d => d.distance))
        {
            var conjuctionOne = conjuctions.FirstOrDefault(c => c.ContainsIndex(indexPointOne));
            var conjuctionTwo = conjuctions.FirstOrDefault(c => c.ContainsIndex(indexPointTwo));

            if (conjuctionOne is not null && conjuctionTwo is not null)
            {
                if (!ReferenceEquals(conjuctionOne, conjuctionTwo))
                {
                    conjuctionTwo.Merge(conjuctionOne);
                    conjuctions.Remove(conjuctionOne);
                }
                if (conjuctions.Count == 1 && conjuctions[0].Size == points.Count)
                {
                    var res = points[indexPointOne].X * (long)points[indexPointTwo].X;
                    Console.WriteLine($"Part Two Result: {res}");
                    return res;
                }
                continue;
            }

            if (conjuctionOne is not null)
            {
                conjuctionOne.AddIndex(indexPointTwo);
                if (conjuctions.Count == 1 && conjuctions[0].Size == points.Count)
                {
                    var res = points[indexPointOne].X * (long)points[indexPointTwo].X;
                    Console.WriteLine($"Part Two Result: {res}");
                    return res;
                }
                continue;
            }

            if (conjuctionTwo is not null)
            {
                conjuctionTwo.AddIndex(indexPointOne);
                if (conjuctions.Count == 1 && conjuctions[0].Size == points.Count)
                {
                    var res = points[indexPointOne].X * (long)points[indexPointTwo].X;
                    Console.WriteLine($"Part Two Result: {res}");
                    return res;
                }
                continue;
            }

            var conj = new Conjuction();
            conj.AddCircut([indexPointOne, indexPointTwo]);
            conjuctions.Add(conj);
        }

        conjuctions.Sort();
        var largestThree = conjuctions.OrderByDescending(c => c.Size).Take(3).ToList();

        var result = largestThree.Aggregate(1, (acc, c) => acc * c.Size);
        Console.WriteLine($"Part One Result: {result}");
        return result;
    }

    private static List<Point> ParseInput(string input)
    {
        var points = new List<Point>();
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length != 3)
                continue;

            if (
                int.TryParse(parts[0], out int x)
                && int.TryParse(parts[1], out int y)
                && int.TryParse(parts[2], out int z)
            )
            {
                points.Add(new Point(x, y, z));
            }
        }
        return points;
    }

    private static long DistanceSquared(Point p1, Point p2)
    {
        long dx = p1.X - p2.X;
        long dy = p1.Y - p2.Y;
        long dz = p1.Z - p2.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    public class Point(int X, int Y, int Z)
    {
        public int X { get; } = X;
        public int Y { get; } = Y;
        public int Z { get; } = Z;

        public override bool Equals(object? obj)
        {
            if (obj is Point other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"Point({X}, {Y}, {Z})";
    }

    public class Conjuction : IComparable<Conjuction>
    {
        public HashSet<int> Indices { get; set; } = [];

        public int Size => Indices.Count;

        public void AddCircut(IEnumerable<int> indices)
        {
            foreach (var i in indices)
            {
                Indices.Add(i);
            }
        }

        public void AddIndex(int index)
        {
            Indices.Add(index);
        }

        public int CompareTo(Conjuction? other)
        {
            return other == null ? 1 : Size.CompareTo(other.Size);
        }

        public bool ContainsIndex(int index) => Indices.Contains(index);

        public void Merge(Conjuction other)
        {
            foreach (var i in other.Indices)
            {
                Indices.Add(i);
            }
        }
    }
}
