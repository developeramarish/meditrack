using Clara.API.Application.Models;
using Clara.API.Application.Validations;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Clara.UnitTests.Validations;

public sealed class StartSessionRequestValidatorTests
{
    private readonly StartSessionRequestValidator _validator = new();

    [Theory]
    [InlineData("Consultation")]
    [InlineData("Follow-up")]
    [InlineData("Review")]
    public void Validate_WithValidSessionType_ShouldPass(string sessionType)
    {
        var request = new StartSessionRequest { SessionType = sessionType };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.SessionType);
    }

    [Theory]
    [InlineData("consultation")]
    [InlineData("follow-up")]
    [InlineData("REVIEW")]
    public void Validate_WithCaseInsensitiveSessionType_ShouldPass(string sessionType)
    {
        var request = new StartSessionRequest { SessionType = sessionType };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.SessionType);
    }

    [Theory]
    [InlineData("")]
    [InlineData("InvalidType")]
    [InlineData("Checkup")]
    public void Validate_WithInvalidSessionType_ShouldFail(string sessionType)
    {
        var request = new StartSessionRequest { SessionType = sessionType };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.SessionType);
    }

    [Fact]
    public void Validate_WithNullPatientId_ShouldPass()
    {
        var request = new StartSessionRequest { PatientId = null, SessionType = "Consultation" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.PatientId);
    }

    [Fact]
    public void Validate_WithValidPatientId_ShouldPass()
    {
        var request = new StartSessionRequest { PatientId = "patient-123", SessionType = "Consultation" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.PatientId);
    }

    [Fact]
    public void Validate_WithTooLongPatientId_ShouldFail()
    {
        var request = new StartSessionRequest
        {
            PatientId = new string('x', 129),
            SessionType = "Consultation"
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.PatientId);
    }
}
