using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoProcessing.VideoOrchestrator.Application.Ports;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using VideoProcessing.VideoOrchestrator.Infra.CrossCutting.Settings;
using VideoProcessing.VideoOrchestrator.Infra.Data.AwsServices;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class StepFunctionServiceTests
{
    private static StepFunctionPayload CreatePayload() =>
        new(
            "exec-1",
            new VideoProcessingInput("v1", "u1", "Title", "bucket", "key", "prefix/", [new VideoChunk(0, "path/")]),
            new ZipOutputInfo("bucket", "videos/u1/v1/output.zip"),
            new UserInfo("User", "user@x.com")
        );

    [Fact]
    public async Task StartExecutionAsync_WhenSdkSucceeds_ReturnsExecutionArn()
    {
        var stepFunctionsMock = new Mock<IAmazonStepFunctions>();
        stepFunctionsMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StartExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StartExecutionResponse { ExecutionArn = "arn:aws:states:us-east-1:123:execution:sm:exec-1" });

        var options = Options.Create(new StepFunctionOptions { StateMachineArn = "arn:aws:states:us-east-1:123:stateMachine:sm" });
        var logger = new Mock<ILogger<StepFunctionService>>().Object;
        var sut = new StepFunctionService(stepFunctionsMock.Object, options, logger);

        var result = await sut.StartExecutionAsync(CreatePayload());

        result.Should().Be("arn:aws:states:us-east-1:123:execution:sm:exec-1");
    }

    [Fact]
    public async Task StartExecutionAsync_WhenSdkThrows_PropagatesException()
    {
        var stepFunctionsMock = new Mock<IAmazonStepFunctions>();
        stepFunctionsMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StartExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonStepFunctionsException("SDK error"));

        var options = Options.Create(new StepFunctionOptions { StateMachineArn = "arn:aws:states:us-east-1:123:stateMachine:sm" });
        var logger = new Mock<ILogger<StepFunctionService>>().Object;
        var sut = new StepFunctionService(stepFunctionsMock.Object, options, logger);

        var act = () => sut.StartExecutionAsync(CreatePayload());

        await act.Should().ThrowAsync<AmazonStepFunctionsException>().WithMessage("*SDK error*");
    }

    [Fact]
    public async Task StartExecutionAsync_VerifyInputJsonContainsVideoId()
    {
        StartExecutionRequest? captured = null;
        var stepFunctionsMock = new Mock<IAmazonStepFunctions>();
        stepFunctionsMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StartExecutionRequest>(), It.IsAny<CancellationToken>()))
            .Callback<StartExecutionRequest, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(new StartExecutionResponse { ExecutionArn = "arn:exec" });

        var options = Options.Create(new StepFunctionOptions { StateMachineArn = "arn:sm" });
        var logger = new Mock<ILogger<StepFunctionService>>().Object;
        var sut = new StepFunctionService(stepFunctionsMock.Object, options, logger);

        await sut.StartExecutionAsync(CreatePayload());

        captured.Should().NotBeNull();
        captured!.Input.Should().Contain("v1");
        captured.Input.Should().Contain("u1");
        captured.Input.Should().Contain("user@x.com");
        captured.Input.Should().Contain("chunk");
        captured.Input.Should().Contain("output.zip");
    }

    [Fact]
    public async Task StartExecutionAsync_VerifyStateMachineArnFromOptions()
    {
        StartExecutionRequest? captured = null;
        var stepFunctionsMock = new Mock<IAmazonStepFunctions>();
        stepFunctionsMock
            .Setup(x => x.StartExecutionAsync(It.IsAny<StartExecutionRequest>(), It.IsAny<CancellationToken>()))
            .Callback<StartExecutionRequest, CancellationToken>((req, _) => captured = req)
            .ReturnsAsync(new StartExecutionResponse { ExecutionArn = "arn:exec" });

        const string expectedArn = "arn:aws:states:us-east-1:999:stateMachine:MyMachine";
        var options = Options.Create(new StepFunctionOptions { StateMachineArn = expectedArn });
        var logger = new Mock<ILogger<StepFunctionService>>().Object;
        var sut = new StepFunctionService(stepFunctionsMock.Object, options, logger);

        await sut.StartExecutionAsync(CreatePayload());

        captured.Should().NotBeNull();
        captured!.StateMachineArn.Should().Be(expectedArn);
    }
}
