using Microsoft.Extensions.Configuration;

public class Question8 : QuestionBase, IQuestion
{
    public int Number => 8;

    public Question8(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        var commands = GetLrCommands(input);
        var map = GetMapValues(input);
        int steps = GetStepsToDestination(map, commands, "AAA", "ZZZ");
        return steps.ToString();
    }

    private char[] GetLrCommands(List<string> input)
    {
        return input[0].ToArray();
    }

    private Dictionary<string, ValueMap> GetMapValues(List<string> input)
    {
        Dictionary<string, ValueMap> dict = new();

        var mapInput = input.Skip(2);
        foreach (var line in mapInput)
        {
            var key = line.Split("=")[0].Trim();
            var leftValue = line.Split("=")[1].Split(",")[0].Split("(")[1].Trim();
            var rightValue = line.Split("=")[1].Split(",")[1].Split(")")[0].Trim();
            dict.Add(key, new ValueMap
            {
                Left = leftValue,
                Right = rightValue
            });
        }

        return dict;
    }

    private int GetStepsToDestination(Dictionary<string, ValueMap> map, char[] commands, string start, string destination)
    {
        string nextKey = start;
        int count = 0;
        while (nextKey != destination)
        {
            foreach (var command in commands)
            {
                ValueMap mapValues = map[nextKey];
                if (nextKey.Equals(destination)) return count;

                nextKey = command.Equals('L') ? mapValues.Left : mapValues.Right;
                count++;
            }
        }

        return count;
    }

    private record ValueMap
    {
        public string Left { get; init; }
        public string Right { get; init; }
    }
}