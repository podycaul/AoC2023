interface IQuestion
{
    bool ShouldRun { get; }
    int Number { get; }
    Task<string> Run();
}