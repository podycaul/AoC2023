using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .Scan(scan => scan
        .FromCallingAssembly()
        .AddClasses(classes => classes.AssignableTo(typeof(IQuestion)))
        .AsImplementedInterfaces()
        .WithSingletonLifetime())
    .AddSingleton<IConfiguration>(config => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .BuildServiceProvider();

var questions = serviceProvider.GetRequiredService<IEnumerable<IQuestion>>();

foreach (var question in questions.Where(x => x.ShouldRun).ToList())
{
    string answer = await question.Run();
    Console.WriteLine($"Question {question.Number}) {answer}");
}