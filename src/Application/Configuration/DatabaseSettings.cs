namespace Application.Configuration;

public sealed class DatabaseSettings
{
    public const string SectionName = "Database";
    public string ConnectionString { get; init; } = string.Empty;
    public bool EnableSensitiveDataLogging { get; init; }
}
