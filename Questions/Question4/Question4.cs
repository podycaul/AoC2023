
using Microsoft.Extensions.Configuration;

// answer: 25183
public class Question4 : QuestionBase, IQuestion
{
    public int Number => 4;

    public Question4(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        int sum = 0;
        for (int i = 0; i < input.Count; i++)
        {
            List<int> intersects = GetIntersections(input[i]);
            sum += ToDouble(intersects.Count);
        }

        return sum.ToString();
    }

    private List<int> GetIntersections(string line)
    {
        List<int> winningList = new();
        List<int> totalNumbers = new();
        string[] numbersSplit = line.Split(":")[1].Split("|");

        winningList = numbersSplit[0].Trim().Split(" ").ToList().Where(x => !string.IsNullOrEmpty(x)).Select(n => int.Parse(n.ToString())).ToList();
        totalNumbers = numbersSplit[1].Trim().Split(" ").ToList().Where(x => !string.IsNullOrEmpty(x)).Select(n => int.Parse(n.ToString())).ToList();

        return winningList.Intersect(totalNumbers).ToList();
    }

    private int ToDouble(int times)
    {
        if (times == 0) return 0;
        int result = 1;
        for (int i = 1; i < times; i++)
        {
            result = result * 2;
        }
        return result;
    }
}