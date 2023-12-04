
using Microsoft.Extensions.Configuration;

public class Question2 : QuestionBase, IQuestion
{
    public int Number => 2;


    public Question2(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = await base.GetQuestionInput();
        return "Not Answered";
    }
}