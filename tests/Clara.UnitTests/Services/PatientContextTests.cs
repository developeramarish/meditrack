using Clara.API.Services;
using FluentAssertions;
using Xunit;

namespace Clara.UnitTests.Services;

public sealed class PatientContextTests
{
    [Fact]
    public void ToPromptSection_WithAllFields_ShouldContainHeader()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = 45,
            Gender = "Male",
            Allergies = ["Penicillin", "Sulfa"],
            ActiveMedications = ["Lisinopril 10mg"],
            ChronicConditions = ["Hypertension"],
            RecentVisitReason = "Annual checkup"
        };
        var result = context.ToPromptSection();
        result.Should().StartWith("## Patient Information");
    }

    [Fact]
    public void ToPromptSection_WithAllFields_ShouldContainAge()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = 45,
            Gender = "Male",
            Allergies = ["Penicillin"],
            ActiveMedications = ["Lisinopril 10mg"],
            ChronicConditions = ["Hypertension"],
            RecentVisitReason = "Annual checkup"
        };
        var result = context.ToPromptSection();
        result.Should().Contain("Age: 45");
    }

    [Fact]
    public void ToPromptSection_WithAllFields_ShouldContainAllergies()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = 45,
            Allergies = ["Penicillin", "Sulfa"]
        };
        var result = context.ToPromptSection();
        result.Should().Contain("Allergies: Penicillin, Sulfa");
    }

    [Fact]
    public void ToPromptSection_WithNoOptionalFields_ShouldReturnEmpty()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = null,
            Gender = null
        };
        var result = context.ToPromptSection();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToPromptSection_WithOnlyAge_ShouldReturnAgeOnly()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = 30
        };
        var result = context.ToPromptSection();
        result.Should().Contain("Age: 30");
        result.Should().NotContain("Gender");
        result.Should().NotContain("Allergies");
    }

    [Fact]
    public void ToPromptSection_WithEmptyLists_ShouldOmitListSections()
    {
        var context = new PatientContext
        {
            PatientId = "p-123",
            Age = 25,
            Allergies = [],
            ActiveMedications = [],
            ChronicConditions = []
        };
        var result = context.ToPromptSection();
        result.Should().Contain("Age: 25");
        result.Should().NotContain("Allergies");
        result.Should().NotContain("Medications");
    }
}
