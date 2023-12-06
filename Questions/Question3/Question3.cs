
using Microsoft.Extensions.Configuration;

// answer: 535351
public class Question3 : QuestionBase, IQuestion
{
    public int Number => 3;

    public Question3(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        List<int> partNumbers = new();
        for (int i = 0; i < input.Count; i++)
        {
            string currentLine = input[i];
            var numbers = GetNumbers(currentLine);
            numbers = numbers.Select(n => { n.LineNumber = i; return n; }).ToList();
            foreach (var partNumber in numbers)
            {
                if (!string.IsNullOrEmpty(AdjacentSymbol(partNumber, input)?.ToString())) partNumbers.Add(partNumber.Number);
            }
        }

        return partNumbers.Sum().ToString();
    }

    private char? AdjacentSymbol(PartNumber partNumber, List<string> grid)
    {
        var coordinates = GetCoordinates(partNumber, grid.Count, grid[0].Length);
        foreach (var coordinate in coordinates)
        {
            var symbol = grid[coordinate.Key][coordinate.Value];
            if (IsSymbol(symbol)) return symbol;
        }
        return null;
    }

    private List<KeyValuePair<int, int>> GetCoordinates(PartNumber partNumber, int gridLength, int lineLength)
    {
        int partNumberLength = partNumber.Number.ToString().Length;
        List<KeyValuePair<int, int>> coordinates = new();
        for (int i = 0; i < partNumberLength; i++)
        {
            // above the number
            if (partNumber.LineNumber > 0)
                coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber - 1, partNumber.Position + i));

            // below the number
            if (partNumber.LineNumber < gridLength - 1)
                coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber + 1, partNumber.Position + i));
        }

        // left and right of the number
        // left
        if (partNumber.Position > 0)
        {
            if (partNumber.LineNumber > 0) coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber - 1, partNumber.Position - 1));
            coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber, partNumber.Position - 1));
            if (partNumber.LineNumber < gridLength - 1) coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber + 1, partNumber.Position - 1));
        }

        // right
        if (partNumber.Position < lineLength - 1)
        {
            if (partNumber.LineNumber > 0) coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber - 1, partNumber.Position + partNumberLength));
            coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber, partNumber.Position + partNumberLength));
            if (partNumber.LineNumber < gridLength - 1) coordinates.Add(new KeyValuePair<int, int>(partNumber.LineNumber + 1, partNumber.Position + partNumberLength));
        }

        return coordinates;
    }

    private bool IsSymbol(char character)
    {
        var chars = "0123456789.";
        return !chars.Any(c => c == character);
    }

    private List<PartNumber> GetNumbers(string line)
    {
        List<PartNumber> numbers = new();
        string number = string.Empty;
        for (int i = 0; i < line.Length; i++)
        {
            bool validNumber = int.TryParse(line[i].ToString(), out int foundDigit);
            if (validNumber) number += foundDigit;

            if (!validNumber || i == line.Length - 1)
            {
                if (!string.IsNullOrEmpty(number)) numbers.Add(new PartNumber
                {
                    Position = Math.Abs(number.Length - i),
                    Number = int.Parse(number)
                });
                // reset
                number = string.Empty;
            }
        }

        return numbers;
    }

    private record PartNumber
    {
        public int Number { get; init; }
        public int Position { get; init; }
        public int LineNumber { get; set; }
    }
}