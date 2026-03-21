using Clara.API.Services;
using FluentAssertions;
using Xunit;

namespace Clara.UnitTests.Services;

public sealed class KnowledgeSeederServiceTests
{
    [Fact]
    public void ChunkText_WithShortContent_ShouldReturnSingleChunk()
    {
        var content = "This is a short document with few words.";
        var chunks = KnowledgeSeederService.ChunkText(content, chunkSize: 500, chunkOverlap: 100);
        chunks.Should().HaveCount(1);
        chunks[0].Should().Contain("short document");
    }

    [Fact]
    public void ChunkText_WithLongContent_ShouldReturnMultipleChunks()
    {
        var words = Enumerable.Range(1, 800).Select(i => $"word{i}");
        var content = string.Join(" ", words);
        var chunks = KnowledgeSeederService.ChunkText(content, chunkSize: 500, chunkOverlap: 100);
        chunks.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ChunkText_WithOverlap_ShouldHaveOverlappingContent()
    {
        var words = Enumerable.Range(1, 800).Select(i => $"word{i}");
        var content = string.Join(" ", words);
        var chunks = KnowledgeSeederService.ChunkText(content, chunkSize: 500, chunkOverlap: 100);
        chunks.Should().HaveCountGreaterThan(1);

        var chunk1Words = chunks[0].Split(' ');
        var chunk2Words = chunks[1].Split(' ');
        var overlapWords = (int)(100 / 1.3);
        var chunk1Tail = chunk1Words.TakeLast(overlapWords).ToArray();
        var chunk2Head = chunk2Words.Take(overlapWords).ToArray();
        chunk1Tail.Should().BeEquivalentTo(chunk2Head);
    }

    [Fact]
    public void ChunkText_WithEmptyContent_ShouldReturnEmptyList()
    {
        var chunks = KnowledgeSeederService.ChunkText("", chunkSize: 500, chunkOverlap: 100);
        chunks.Should().BeEmpty();
    }

    [Fact]
    public void ChunkText_WithWhitespaceOnly_ShouldReturnEmptyList()
    {
        var chunks = KnowledgeSeederService.ChunkText("   \n\t  ", chunkSize: 500, chunkOverlap: 100);
        chunks.Should().BeEmpty();
    }

    [Theory]
    [InlineData("CDC-ChestPain.txt", "cdc")]
    [InlineData("AHA-HeartFailure.txt", "aha")]
    [InlineData("WHO-EssentialMedicines.txt", "who")]
    [InlineData("NICE-Hypertension.txt", "nice")]
    [InlineData("FDA-DrugInteractions.txt", "fda")]
    public void ExtractCategory_WithKnownPrefix_ShouldReturnLowercaseCategory(string fileName, string expectedCategory)
    {
        var category = KnowledgeSeederService.ExtractCategory(fileName);
        category.Should().Be(expectedCategory);
    }

    [Theory]
    [InlineData("chest-pain-assessment.md")]
    [InlineData("random-document.txt")]
    public void ExtractCategory_WithNoKnownPrefix_ShouldReturnNull(string fileName)
    {
        var category = KnowledgeSeederService.ExtractCategory(fileName);
        category.Should().BeNull();
    }
}
