using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Refit;
using VideoProcessing.VideoOrchestrator.Domain.Exceptions;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using VideoProcessing.VideoOrchestrator.Infra.Data.ExternalApis.M2MAuth;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class M2MTokenServiceTests
{
    private readonly Mock<IM2MAuthApi> _apiMock = new();
    private readonly Mock<ILogger<M2MTokenService>> _loggerMock = new();
    private readonly IOptions<M2MAuthOptions> _options;

    public M2MTokenServiceTests()
    {
        _options = Options.Create(new M2MAuthOptions
        {
            TokenEndpoint = "https://auth.example.com/token",
            ClientId = "client-id",
            ClientSecret = "secret-value"
        });
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenApiReturnsToken_ReturnsAccessToken()
    {
        var sut = new M2MTokenService(_apiMock.Object, _options, _loggerMock.Object);
        _apiMock
            .Setup(x => x.GetTokenAsync(It.IsAny<M2MTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M2MTokenResponse { AccessToken = "token-123", ExpiresIn = 3600 });

        var result = await sut.GetAccessTokenAsync();

        result.Should().Be("token-123");
    }

    [Fact]
    public async Task GetAccessTokenAsync_When401_ThrowsDomainException()
    {
        var sut = new M2MTokenService(_apiMock.Object, _options, _loggerMock.Object);
        _apiMock
            .Setup(x => x.GetTokenAsync(It.IsAny<M2MTokenRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(await ApiException.Create(
                new HttpRequestMessage(),
                HttpMethod.Post,
                new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized),
                null!));

        var act = () => sut.GetAccessTokenAsync();

        await act.Should().ThrowAsync<ExternalServiceException>();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenApiFails_LoggerDoesNotReceiveClientSecretOrToken()
    {
        var sut = new M2MTokenService(_apiMock.Object, _options, _loggerMock.Object);
        _apiMock
            .Setup(x => x.GetTokenAsync(It.IsAny<M2MTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M2MTokenResponse { AccessToken = "sensitive-token", ExpiresIn = 3600 });

        await sut.GetAccessTokenAsync();

        var sensitive = new[] { "secret-value", "sensitive-token" };
        foreach (var inv in _loggerMock.Invocations)
        {
            var text = string.Join(" ", inv.Arguments.Select(a => a?.ToString() ?? ""));
            foreach (var s in sensitive)
                text.Should().NotContain(s);
        }
    }
}
