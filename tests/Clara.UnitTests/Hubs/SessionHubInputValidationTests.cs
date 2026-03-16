using Clara.API.Hubs;
using Clara.API.Services;
using FluentAssertions;
using Xunit;

namespace Clara.UnitTests.Hubs;

public sealed class SessionHubInputValidationTests
{
    [Theory]
    [InlineData("Doctor")]
    [InlineData("Patient")]
    public void IsValidSpeaker_WithValidRole_ShouldReturnTrue(string speaker)
    {
        SessionHubValidation.IsValidSpeaker(speaker).Should().BeTrue();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("System")]
    [InlineData("")]
    [InlineData("Doctor; DROP TABLE sessions;")]
    public void IsValidSpeaker_WithInvalidRole_ShouldReturnFalse(string speaker)
    {
        SessionHubValidation.IsValidSpeaker(speaker).Should().BeFalse();
    }

    [Fact]
    public void IsValidTranscriptText_WithReasonableLength_ShouldReturnTrue()
    {
        SessionHubValidation.IsValidTranscriptText(new string('a', 2000)).Should().BeTrue();
    }

    [Fact]
    public void IsValidTranscriptText_WithExcessiveLength_ShouldReturnFalse()
    {
        SessionHubValidation.IsValidTranscriptText(new string('a', 5001)).Should().BeFalse();
    }

    [Fact]
    public void IsValidTranscriptText_WithEmptyText_ShouldReturnFalse()
    {
        SessionHubValidation.IsValidTranscriptText("").Should().BeFalse();
    }

    [Fact]
    public void IsValidAudioChunkSize_WithReasonableSize_ShouldReturnTrue()
    {
        SessionHubValidation.IsValidAudioChunkSize(1_000_000).Should().BeTrue();
    }

    [Fact]
    public void IsValidAudioChunkSize_WithExcessiveSize_ShouldReturnFalse()
    {
        SessionHubValidation.IsValidAudioChunkSize(11_000_000).Should().BeFalse();
    }

    [Fact]
    public void IsValidAudioChunkSize_WithZeroSize_ShouldReturnFalse()
    {
        SessionHubValidation.IsValidAudioChunkSize(0).Should().BeFalse();
    }
}
