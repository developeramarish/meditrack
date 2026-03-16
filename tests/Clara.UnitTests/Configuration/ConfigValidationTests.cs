using Clara.API.Extensions;
using FluentAssertions;
using Xunit;

namespace Clara.UnitTests.Configuration;

public sealed class ConfigValidationTests
{
    [Theory]
    [InlineData("REPLACE_IN_OVERRIDE")]
    [InlineData("sk-placeholder-for-dev")]
    [InlineData("placeholder-for-dev")]
    [InlineData("")]
    [InlineData("   ")]
    public void IsRealApiKey_WithPlaceholderValue_ShouldReturnFalse(string value)
    {
        ConfigValidator.IsRealApiKey(value).Should().BeFalse();
    }

    [Fact]
    public void IsRealApiKey_WithNull_ShouldReturnFalse()
    {
        ConfigValidator.IsRealApiKey(null).Should().BeFalse();
    }

    [Fact]
    public void IsRealApiKey_WithRealKey_ShouldReturnTrue()
    {
        ConfigValidator.IsRealApiKey("sk-proj-abc123def456").Should().BeTrue();
    }
}
