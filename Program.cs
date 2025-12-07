using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  
""";
        var input = Seeder.DaySix();
        var result = DaySixSolutions.SolvePartTwo(input);
    }
}
