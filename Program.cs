using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
0:
###
##.
##.

1:
###
##.
.##

2:
.##
###
##.

3:
##.
###
##.

4:
###
#..
###

5:
###
.#.
###

4x4: 0 0 0 0 2 0
12x5: 1 0 1 0 2 2
12x5: 1 0 1 0 3 2
""";
        var input = Seeder.DayTwelve();
        var partOne = DayTwelveSolutions.SolvePartOne(input);
        System.Console.WriteLine(partOne);
    }
}
