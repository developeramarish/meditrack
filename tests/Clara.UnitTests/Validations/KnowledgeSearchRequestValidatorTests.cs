using Clara.API.Apis;
using Clara.API.Application.Validations;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Clara.UnitTests.Validations;

public sealed class KnowledgeSearchRequestValidatorTests
{
    private readonly KnowledgeSearchRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldPass()
    {
        var request = new KnowledgeSearchRequest { Query = "chest pain symptoms" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyQuery_ShouldFail(string query)
    {
        var request = new KnowledgeSearchRequest { Query = query };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Query);
    }

    [Fact]
    public void Validate_WithQueryExceeding1000Chars_ShouldFail()
    {
        var request = new KnowledgeSearchRequest { Query = new string('x', 1001) };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Query);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Validate_WithInvalidTopK_ShouldFail(int topK)
    {
        var request = new KnowledgeSearchRequest { Query = "test", TopK = topK };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.TopK);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Validate_WithValidTopK_ShouldPass(int topK)
    {
        var request = new KnowledgeSearchRequest { Query = "test", TopK = topK };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.TopK);
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(1.1f)]
    public void Validate_WithInvalidMinScore_ShouldFail(float minScore)
    {
        var request = new KnowledgeSearchRequest { Query = "test", MinScore = minScore };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.MinScore);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void Validate_WithValidMinScore_ShouldPass(float minScore)
    {
        var request = new KnowledgeSearchRequest { Query = "test", MinScore = minScore };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.MinScore);
    }
}
