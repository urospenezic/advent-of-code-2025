using System;

namespace MyScript;

public class DayTenSolutions
{
    public static int SolvePartOne(string input)
    {
        var machines = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(Machine.Parse)
            .ToList();
        var firstMachine = machines[0];

        var result = machines.Sum(SolvePartOneForMachine);
        return result;
    }

    public static int SolvePartTwo(string input)
    {
        var machines = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(Machine.Parse)
            .ToList();

        return machines.Sum(SolvePartTwoForMachine);
    }

    private static int SolvePartOneForMachine(Machine machine)
    {
        //bfs to find shortest path to target mask walking via xor
        var targetMask = machine.LightMask;
        var visited = new HashSet<int>();
        var queue = new Queue<(int current, int presses)>([(0, 0)]);

        while (queue.Count > 0)
        {
            var (current, presses) = queue.Dequeue();
            if (current == targetMask)
            {
                return presses;
            }

            foreach (var buttonMask in machine.ButtonMasks)
            {
                var next = current ^ buttonMask;
                if (visited.Add(next))
                {
                    queue.Enqueue((next, presses + 1));
                }
            }
        }

        return 0;
    }

    private static int SolvePartTwoForMachine(Machine machine)
    {
        var target = machine.Joltages;
        int numCounters = target.Length;

        var buttonMasks = machine.ButtonMasks;
        int numButtons = buttonMasks.Length;

        // Build matrix: each row is an equation, columns 0..numButtons-1 are
        // button coefficients (0 or 1), and column numButtons is the target.
        var mat = new long[numCounters][];
        for (int cnt = 0; cnt < numCounters; cnt++)
        {
            mat[cnt] = new long[numButtons + 1];
            mat[cnt][numButtons] = target[cnt];
        }

        for (int btn = 0; btn < numButtons; btn++)
        {
            int mask = buttonMasks[btn];
            for (int cnt = 0; cnt < numCounters; cnt++)
            {
                if ((mask & (1 << cnt)) != 0)
                {
                    mat[cnt][btn] = 1;
                }
            }
        }

        // Compute simple upper bounds for each button variable: at most the
        // minimum target among counters it affects.
        var bounds = new long?[numButtons];
        for (int cnt = 0; cnt < numCounters; cnt++)
        {
            long t = mat[cnt][numButtons];
            for (int btn = 0; btn < numButtons; btn++)
            {
                if (mat[cnt][btn] == 1)
                {
                    if (bounds[btn] is null || bounds[btn] > t)
                    {
                        bounds[btn] = t;
                    }
                }
            }
        }

        // Reduce the system of equations via integer Gaussian elimination.
        var remaining = new List<long[]>(mat);
        var reduced = new List<long[]>();

        static long Gcd(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                long tmp = a % b;
                a = b;
                b = tmp;
            }
            return a;
        }

        static long RowGcd(long[] row)
        {
            long g = 0;
            for (int i = 0; i < row.Length; i++)
            {
                g = Gcd(g, row[i]);
            }
            return g;
        }

        while (remaining.Count > 0)
        {
            var curRow = remaining[remaining.Count - 1];
            remaining.RemoveAt(remaining.Count - 1);

            int selectedBtn = -1;
            for (int btn = 0; btn < numButtons; btn++)
            {
                if (curRow[btn] != 0)
                {
                    selectedBtn = btn;
                    break;
                }
            }

            if (selectedBtn < 0)
            {
                // All coefficients zero; equation is either 0 = 0 (ignore) or
                // inconsistent 0 = c (which should not happen for valid input).
                if (curRow[numButtons] != 0)
                {
                    return 0;
                }
                continue;
            }

            long a = curRow[selectedBtn];

            for (int r = 0; r < remaining.Count; r++)
            {
                var row = remaining[r];
                long b = row[selectedBtn];

                // row := row * a - curRow * b so that row[selectedBtn] becomes 0.
                for (int i = 0; i < numButtons + 1; i++)
                {
                    row[i] = row[i] * a - curRow[i] * b;
                }

                long rowGcd = RowGcd(row);
                if (rowGcd != 0)
                {
                    if (row[numButtons] < 0)
                    {
                        rowGcd = -rowGcd;
                    }

                    for (int i = 0; i < numButtons + 1; i++)
                    {
                        row[i] /= rowGcd;
                    }
                }
            }

            reduced.Add(curRow);
        }

        // Try to deduce variable values from the reduced system given some
        // currently known values; return null if contradiction.
        long?[]? Substitute(long?[] known)
        {
            var result = (long?[])known.Clone();

            foreach (var row in reduced)
            {
                int unknownCount = 0;
                int unknownIdx = -1;
                long sumKnown = 0;
                long targetVal = row[numButtons];

                for (int i = 0; i < numButtons; i++)
                {
                    long coef = row[i];
                    var val = result[i];
                    if (!val.HasValue)
                    {
                        if (coef != 0)
                        {
                            unknownCount++;
                            unknownIdx = i;
                        }
                    }
                    else
                    {
                        sumKnown += val.Value * coef;
                    }
                }

                if (unknownCount == 0)
                {
                    if (sumKnown != targetVal)
                    {
                        return null;
                    }
                }
                else if (unknownCount == 1)
                {
                    long rhs = targetVal - sumKnown;
                    long coef = row[unknownIdx];
                    if (coef == 0 || rhs % coef != 0)
                    {
                        return null;
                    }

                    long value = rhs / coef;
                    if (value < 0)
                    {
                        return null;
                    }

                    result[unknownIdx] = value;
                }
            }

            return result;
        }

        long[]? bestSolution = null;
        long bestSum = 0;

        void Search(long?[] known)
        {
            var substituted = Substitute(known);
            if (substituted is null)
            {
                return;
            }

            long partialSum = 0;
            bool allKnown = true;
            for (int i = 0; i < numButtons; i++)
            {
                if (substituted[i].HasValue)
                {
                    partialSum += substituted[i]!.Value;
                }
                else
                {
                    allKnown = false;
                }
            }

            if (bestSolution is not null && partialSum > bestSum)
            {
                return;
            }

            if (allKnown)
            {
                if (bestSolution is null || partialSum < bestSum)
                {
                    bestSolution = substituted.Select(v => v!.Value).ToArray();
                    bestSum = partialSum;
                }
                return;
            }

            // Select the variable that appears in the fewest equations with
            // unknowns (heuristic to reduce branching).
            int selectedBtn = -1;
            int bestUnknownCount = numButtons + 1;

            foreach (var row in reduced)
            {
                int unknownCount = 0;
                int candidateIdx = -1;

                for (int i = 0; i < numButtons; i++)
                {
                    if (!substituted[i].HasValue && row[i] != 0)
                    {
                        unknownCount++;
                        candidateIdx = i;
                    }
                }

                if (unknownCount > 0 && unknownCount < bestUnknownCount)
                {
                    bestUnknownCount = unknownCount;
                    selectedBtn = candidateIdx;
                }
            }

            if (selectedBtn < 0)
            {
                return;
            }

            long upper = bounds[selectedBtn] ?? 0L;

            for (long val = 0; val <= upper; val++)
            {
                var next = (long?[])substituted.Clone();
                next[selectedBtn] = val;
                Search(next);
            }
        }

        var initialKnown = new long?[numButtons];
        Search(initialKnown);

        if (bestSolution is null)
        {
            return 0;
        }

        return (int)bestSum;
    }
}

public class Machine(string lights, List<int[]> buttons, int[] joltages)
{
    public int LightMask { get; } = lights.Select((c, i) => c == '#' ? 1 << i : 0).Sum();

    public int[] ButtonMasks { get; } =
    [.. buttons.Select(button => button.Select(i => 1 << i).Sum())];

    public int[] Joltages { get; } = joltages;

    public static Machine Parse(string line)
    {
        var parts = line.Split(' ');
        return new Machine(
            lights: parts[0][1..^1],
            buttons: [.. parts[1..^1].Select(Helpers.ParseInts)],
            joltages: Helpers.ParseInts(parts[^1])
        );
    }

    public override string ToString()
    {
        return $"[{lights}] "
            + $"({string.Join(") (", buttons.Select(indices => string.Join(',', indices)))}) "
            + $"{{{string.Join(',', joltages)}}}";
    }

    public int PressButton(int buttonMask) => LightMask ^ buttonMask;
}

public static class Helpers
{
    public static int[] ParseInts(this string s)
    {
        return [.. s[1..^1].Split(',').Select(int.Parse)];
    }
}
