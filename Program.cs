using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3
""";
        var input = Seeder.DayNine();
        var result = DayNineSolutions.SolvePartTwo(input);
    }
}
