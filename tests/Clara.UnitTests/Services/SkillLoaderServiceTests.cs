using Clara.API.Domain;
using Clara.API.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Clara.UnitTests.Services;

public sealed class SkillLoaderServiceTests
{
    private static SkillLoaderService CreateServiceWithSkills(params ClinicalSkill[] skills)
    {
        var config = new ConfigurationBuilder().Build();
        var service = new SkillLoaderService(NullLogger<SkillLoaderService>.Instance, config);
        foreach (var skill in skills)
        {
            service.AddSkillForTesting(skill);
        }
        return service;
    }

    [Fact]
    public void FindMatchingSkill_WithMatchingTrigger_ShouldReturnSkill()
    {
        var chestPainSkill = new ClinicalSkill
        {
            Id = "chest-pain",
            Name = "Chest Pain Assessment",
            Triggers = ["chest pain", "chest discomfort"],
            Priority = 100,
            Content = "HEART Score workflow"
        };
        var service = CreateServiceWithSkills(chestPainSkill);

        var result = service.FindMatchingSkill("[Patient]: I have chest pain");

        result.Should().NotBeNull();
        result!.Id.Should().Be("chest-pain");
    }

    [Fact]
    public void FindMatchingSkill_WithNoMatch_ShouldReturnNull()
    {
        var skill = new ClinicalSkill
        {
            Id = "chest-pain", Name = "Chest Pain", Triggers = ["chest pain"],
            Priority = 100, Content = "Content"
        };
        var service = CreateServiceWithSkills(skill);

        var result = service.FindMatchingSkill("[Patient]: I have a headache");

        result.Should().BeNull();
    }

    [Fact]
    public void FindMatchingSkill_WithMultipleMatches_ShouldReturnHighestPriority()
    {
        var lowPriority = new ClinicalSkill
        {
            Id = "general-triage", Name = "General Triage", Triggers = ["pain"],
            Priority = 10, Content = "Triage content"
        };
        var highPriority = new ClinicalSkill
        {
            Id = "chest-pain", Name = "Chest Pain Assessment", Triggers = ["chest pain"],
            Priority = 100, Content = "Chest pain content"
        };
        var service = CreateServiceWithSkills(lowPriority, highPriority);

        var result = service.FindMatchingSkill("[Patient]: I have chest pain");

        result!.Id.Should().Be("chest-pain");
    }

    [Fact]
    public void FindMatchingSkill_ShouldBeCaseInsensitive()
    {
        var skill = new ClinicalSkill
        {
            Id = "chest-pain", Name = "Chest Pain", Triggers = ["Chest Pain"],
            Priority = 100, Content = "Content"
        };
        var service = CreateServiceWithSkills(skill);

        var result = service.FindMatchingSkill("[Patient]: I have CHEST PAIN");

        result.Should().NotBeNull();
    }

    [Fact]
    public void FindMatchingSkill_WithEmptyConversation_ShouldReturnNull()
    {
        var skill = new ClinicalSkill
        {
            Id = "chest-pain", Name = "Chest Pain", Triggers = ["chest pain"],
            Priority = 100, Content = "Content"
        };
        var service = CreateServiceWithSkills(skill);

        var result = service.FindMatchingSkill("");

        result.Should().BeNull();
    }
}
