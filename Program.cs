using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.
""";
        var input = Seeder.DayFour();
        var result = DayFourSolutions.SolvePartTwo(input, 4);
    }
}
