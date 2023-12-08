
using Microsoft.Extensions.Configuration;

// answer: 41513103
public class Question6 : QuestionBase, IQuestion
{
    public int Number => 6;

    public Question6(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();
        var records = GetRecords(input);
        int totalMarginOfError = 1;
        foreach (var record in records)
        {
            List<double> millis = GetWinningMilliseconds(record);
            totalMarginOfError *= millis.Count;
        }
        return totalMarginOfError.ToString();
    }

    /// <summary>
    /// Determines how many different combinations of milliseconds can be used to win the race
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    private List<double> GetWinningMilliseconds(BoatRecord record)
    {
        List<double> milliseconds = new();
        for (int i = 1; i < record.TimeTaken; i++)
        {
            // i being the time the button is held down
            double timeRemaining = record.TimeTaken - i;
            if ((timeRemaining * i) >= record.DistanceTraveled) milliseconds.Add(i);
        }
        return milliseconds;
    }

    private List<BoatRecord> GetRecords(List<string> input)
    {
        List<BoatRecord> records = new();
        double time = double.Parse(string.Join("", input[0].Split(":")[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList()));
        double distance = double.Parse(string.Join("", input[1].Split(":")[1].Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList()));

        records.Add(new()
        {
            TimeTaken = time,
            DistanceTraveled = distance
        });

        return records;
    }

    private record BoatRecord
    {
        public double TimeTaken { get; init; }
        public double DistanceTraveled { get; init; }
    }
}