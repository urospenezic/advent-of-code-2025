using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
3-5
10-14
16-20
12-18

1
5
8
11
17
32
""";
        var input = Seeder.DayFive();
        var result = DayFiveSolutions.SolvePartTwo(input);
    }
}
