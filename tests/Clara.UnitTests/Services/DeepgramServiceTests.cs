using System.Net;
using Clara.API.Services;
using Clara.UnitTests.TestInfrastructure;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Clara.UnitTests.Services;

public sealed class DeepgramServiceTests
{
    private readonly DeepgramService _service;
    private readonly MockHttpMessageHandler _httpHandler;

    public DeepgramServiceTests()
    {
        _httpHandler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.deepgram.com") };
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("Deepgram").Returns(httpClient);
        _service = new DeepgramService(httpClientFactory, NullLogger<DeepgramService>.Instance);
    }

    [Fact]
    public async Task TranscribeAsync_WithValidAudio_ShouldReturnTranscript()
    {
        _httpHandler.SetResponse(HttpStatusCode.OK, """
        {
            "results": {
                "channels": [{
                    "alternatives": [{
                        "transcript": "I have chest pain",
                        "confidence": 0.95
                    }]
                }]
            }
        }
        """);

        var result = await _service.TranscribeAsync("session-1", new byte[] { 1, 2, 3 });

        result.Should().NotBeNull();
        result!.Transcript.Should().Be("I have chest pain");
        result.Confidence.Should().BeApproximately(0.95f, 0.01f);
    }

    [Fact]
    public async Task TranscribeAsync_WithEmptyAudio_ShouldReturnNull()
    {
        var result = await _service.TranscribeAsync("session-1", []);
        result.Should().BeNull();
    }

    [Fact]
    public async Task TranscribeAsync_WithApiError_ShouldReturnNull()
    {
        _httpHandler.SetResponse(HttpStatusCode.InternalServerError, "Server error");
        var result = await _service.TranscribeAsync("session-1", new byte[] { 1, 2, 3 });
        result.Should().BeNull();
    }

    [Fact]
    public async Task TranscribeAsync_WithEmptyTranscript_ShouldReturnNull()
    {
        _httpHandler.SetResponse(HttpStatusCode.OK, """
        {
            "results": {
                "channels": [{
                    "alternatives": [{
                        "transcript": "",
                        "confidence": 0.0
                    }]
                }]
            }
        }
        """);

        var result = await _service.TranscribeAsync("session-1", new byte[] { 1, 2, 3 });
        result.Should().BeNull();
    }

    [Fact]
    public async Task TranscribeAsync_WithEmptyChannels_ShouldReturnNull()
    {
        _httpHandler.SetResponse(HttpStatusCode.OK, """{ "results": { "channels": [] } }""");
        var result = await _service.TranscribeAsync("session-1", new byte[] { 1, 2, 3 });
        result.Should().BeNull();
    }
}
