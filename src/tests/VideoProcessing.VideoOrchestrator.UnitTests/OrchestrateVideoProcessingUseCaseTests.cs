using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Application.UseCases;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class OrchestrateVideoProcessingUseCaseTests
{
    private readonly Mock<IStepFunctionService> _stepFunctionMock = new();
    private readonly Mock<ILogger<OrchestrateVideoProcessingUseCase>> _loggerMock = new();

    private static VideoDetails CreateDetails() =>
        new("video-1", "user-1", "Title", "Status", "videos/user-1/video-1/original", "my-bucket", "User", "user@example.com");

    [Fact]
    public async Task ExecuteAsync_HappyPath_CallsStepFunctionServiceAndReturnsExecutionArn()
    {
        const string expectedArn = "arn:aws:states:us-east-1:123:execution:sm:exec-1";
        _stepFunctionMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StepFunctionPayload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedArn);

        var sut = new OrchestrateVideoProcessingUseCase(_stepFunctionMock.Object, _loggerMock.Object);

        var result = await sut.ExecuteAsync(CreateDetails());

        result.Should().Be(expectedArn);
        _stepFunctionMock.Verify(
            x => x.StartExecutionAsync(It.IsAny<StepFunctionPayload>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStepFunctionFails_PropagatesException()
    {
        _stepFunctionMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StepFunctionPayload>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Start failed"));

        var sut = new OrchestrateVideoProcessingUseCase(_stepFunctionMock.Object, _loggerMock.Object);

        var act = () => sut.ExecuteAsync(CreateDetails());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Start failed");
    }

    [Fact]
    public async Task ExecuteAsync_VerifyExecutionIdContainsVideoId()
    {
        StepFunctionPayload? captured = null;
        _stepFunctionMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StepFunctionPayload>(), It.IsAny<CancellationToken>()))
            .Callback<StepFunctionPayload, CancellationToken>((p, _) => captured = p)
            .ReturnsAsync("arn:exec");

        var sut = new OrchestrateVideoProcessingUseCase(_stepFunctionMock.Object, _loggerMock.Object);
        var details = CreateDetails();

        await sut.ExecuteAsync(details);

        captured.Should().NotBeNull();
        captured!.ExecutionId.Should().StartWith("exec-");
        captured.ExecutionId.Should().Contain("video-1");
        captured.Video.Chunks.Should().NotBeEmpty();
        captured.Zip.OutputBucket.Should().Be("my-bucket");
        captured.Zip.OutputKey.Should().Contain("output.zip");
    }
}
