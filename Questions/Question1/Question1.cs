
using Microsoft.Extensions.Configuration;

// Correct Answer: 55358
public class Question1 : QuestionBase, IQuestion
{
    public int Number => 1;

    private readonly Dictionary<string, int> _numberDict = new(){
        {"one", 1},
        {"two", 2},
        {"three", 3},
        {"four", 4},
        {"five", 5},
        {"six", 6},
        {"seven", 7},
        {"eight", 8},
        {"nine", 9}
    };

    private readonly HashSet<string> _startingLetter = new HashSet<string>{
        "o", "t", "f", "s", "e", "n"
    };

    public Question1(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        int sum = 0;
        var input = await base.GetQuestionInput();
        foreach (var line in input)
        {
            int? digit1 = null;
            int? digit2 = null;
            for (int i = 0; i < line.Length; i++)
            {
                if (int.TryParse(line[i].ToString(), out int found))
                {
                    digit1 ??= found;
                    digit2 = found;
                    continue;
                }

                // does it start with an appropriate letter?
                if (!_startingLetter.Contains(line[i].ToString())) continue;
                string remainder = line.Substring(i);
                string foundWord = remainder[0].ToString();
                for (int j = 1; j < remainder.Length; j++)
                {
                    foundWord += remainder[j].ToString();
                    if (!_numberDict.ContainsKey(foundWord)) continue;
                    digit1 ??= _numberDict[foundWord];
                    digit2 = _numberDict[foundWord];
                }
            }

            int calibration = int.Parse($"{digit1}{digit2}");

            sum += calibration;
        }

        return $"Sum: {sum}";
    }
}