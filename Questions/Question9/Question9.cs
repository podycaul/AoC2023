
using Microsoft.Extensions.Configuration;

public class Question9 : QuestionBase, IQuestion
{
    public int Number => 9;

    public Question9(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        int sum = 0;
        
        foreach (var line in input)
        {
            var matrix = GetHistoryMatrix(GetNumberValues(line));
            sum += CalculateExtrapolatedValue(matrix);
        }

        return sum.ToString();
    }

    private List<int> GetNumberValues(string line)
    {
        List<int> numbers = new();
        foreach (var character in line.Split(" "))
        {
            if (string.IsNullOrEmpty(character)) continue;
            if (int.TryParse(character.ToString(), out int number)) numbers.Add(number);
        }

        return numbers;
    }

    private List<List<int>> GetHistoryMatrix(List<int> numbers)
    {
        List<List<int>> histories = new()
        {
            numbers
        };
        if (numbers.All(n => n == 0)) return histories;
        List<int> history = new();
        for (int i = 0; i < numbers.Count; i++)
        {
            if (i + 1 >= numbers.Count) break;
            history.Add((numbers[i+1]) - numbers[i]);
        }
        
        histories.AddRange(GetHistoryMatrix(history));
        return histories;
    }

    private int CalculateExtrapolatedValue(List<List<int>> matrix)
    {
        matrix.Reverse();
        int lastSum = 0;
        for (int i = 0; i < matrix.Count; i++)
        {
            if (i + 1 >= matrix.Count) return lastSum;
            lastSum += matrix[i + 1].Last();
        }
        return lastSum;
    }
}