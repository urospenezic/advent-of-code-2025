using MyScript;

public class Program
{
    public static void Main(string[] args)
    {
        var test = """
svr: aaa bbb
aaa: fft
fft: ccc
bbb: tty
tty: ccc
ccc: ddd eee
ddd: hub
hub: fff
eee: dac
dac: fff
fff: ggg hhh
ggg: out
hhh: out
""";
        var input = Seeder.DayEleven();
        var partOne = DayElevenSolutions.SolvePartTwo(input);
        System.Console.WriteLine(partOne);
    }
}
