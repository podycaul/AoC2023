using Microsoft.Extensions.Configuration;

public abstract class QuestionBase
{
    internal int QuestionNumber { get; set; }
    public bool ShouldRun => _configuration.GetValue($"Questions:Question{QuestionNumber}", true);

    private readonly IConfiguration _configuration;
    public QuestionBase(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<string>> GetQuestionInput()
    {
        using var fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "Questions", $"Question{QuestionNumber}", "input.txt"));
        using var sr = new StreamReader(fs);
        List<string> lines = new();
        string line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            lines.Add(line);
        }
        return lines;
    }
}