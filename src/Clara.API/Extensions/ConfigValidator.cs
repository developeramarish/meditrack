namespace Clara.API.Extensions;

/// <summary>
/// Validates configuration values to prevent placeholder secrets in production.
/// </summary>
public static class ConfigValidator
{
    private static readonly string[] PlaceholderValues =
    [
        "REPLACE_IN_OVERRIDE",
        "sk-placeholder-for-dev",
        "placeholder-for-dev"
    ];

    public static bool IsRealApiKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return !PlaceholderValues.Contains(value, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates critical config values on startup. Throws in Production if placeholders remain.
    /// </summary>
    public static void ValidateProductionConfig(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsProduction())
            return;

        var openAiKey = configuration["AI:OpenAI:ApiKey"];
        var deepgramKey = configuration["AI:Deepgram:ApiKey"];

        if (!IsRealApiKey(openAiKey))
            throw new InvalidOperationException(
                "AI:OpenAI:ApiKey is a placeholder. Set a real API key for production.");

        if (!IsRealApiKey(deepgramKey))
            throw new InvalidOperationException(
                "AI:Deepgram:ApiKey is a placeholder. Set a real API key for production.");
    }
}
