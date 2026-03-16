using Clara.API.Services;

namespace Clara.API.Hubs;

internal static class SessionHubValidation
{
    private static readonly HashSet<string> ValidSpeakers = new(StringComparer.Ordinal)
    {
        SpeakerRole.Doctor,
        SpeakerRole.Patient
    };

    public const int MaxTranscriptLength = 5000;
    public const int MaxAudioChunkBytes = 10 * 1024 * 1024;

    public static bool IsValidSpeaker(string speaker)
        => !string.IsNullOrWhiteSpace(speaker) && ValidSpeakers.Contains(speaker);

    public static bool IsValidTranscriptText(string text)
        => !string.IsNullOrWhiteSpace(text) && text.Length <= MaxTranscriptLength;

    public static bool IsValidAudioChunkSize(int byteCount)
        => byteCount > 0 && byteCount <= MaxAudioChunkBytes;
}
