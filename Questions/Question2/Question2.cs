
using Microsoft.Extensions.Configuration;

public class Question2 : QuestionBase, IQuestion
{
    public int Number => 2;
    private readonly int _maxRed = 12;
    private readonly int _maxGreen = 13;
    private readonly int _maxBlue = 14;

    public Question2(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = await base.GetQuestionInput();
        // part 1
        // return GetValidGamesSum(input).ToString();

        // part 2
        return GetPowersOfMinimumSets(input).ToString();
    }

    private int GetPowersOfMinimumSets(IEnumerable<string> input)
    {
        int power = 0;
        foreach (var line in input)
        {
            List<Grab> grabs = GetCubeGrabs(line.Split(":")[1].Trim());
            List<int> minimumNeeded = new();

            foreach (var cubeSet in grabs.SelectMany(x => x.Cubes).GroupBy(g => g.Color))
            {
                minimumNeeded.Add(cubeSet.Max(x => x.Count));
            }

            power += minimumNeeded.Aggregate(1, (x, y) => x * y);
        }

        return power;
    }

    private int GetValidGamesSum(IEnumerable<string> input)
    {
        List<int> validGames = new();
        int gameNumber = 1;

        foreach (var line in input)
        {
            List<Grab> grabs = GetCubeGrabs(line.Split(":")[1].Trim());
            try
            {
                foreach (Grab grab in grabs)
                {
                    foreach (var cubes in grab.Cubes)
                    {
                        switch (cubes.Color)
                        {
                            case Colors.Red when cubes.Count > _maxRed:
                                throw new InvalidGameException();
                            case Colors.Green when cubes.Count > _maxGreen:
                                throw new InvalidGameException();
                            case Colors.Blue when cubes.Count > _maxBlue:
                                throw new InvalidGameException();
                        }
                    }
                }

                validGames.Add(gameNumber);
            }
            catch (InvalidGameException) { }

            gameNumber++;
        }

        return validGames.Sum();
    }

    private List<Grab> GetCubeGrabs(string roundData)
    {
        List<Grab> grabs = new();
        // 3 blue, 4 red; etc...
        string[] rounds = roundData.Split(";");
        foreach (string round in rounds)
        {
            Grab grab = new();
            string[] cubeTypes = round.Split(",");
            foreach (string type in cubeTypes)
            {
                string[] split = type.Trim().Split(" ");
                int amount = int.Parse(split[0].Trim().ToString());
                Colors color = Enum.Parse<Colors>(split[1].Trim(), ignoreCase: true);
                grab.Cubes.Add(new(color, amount));
            }
            grabs.Add(grab);
        }
        return grabs;
    }

    private record Grab
    {
        public List<(Colors Color, int Count)> Cubes = new();
    }

    private enum Colors
    {
        Red = 0,
        Blue = 1,
        Green = 2
    }

    private class InvalidGameException : Exception
    {
    }
}