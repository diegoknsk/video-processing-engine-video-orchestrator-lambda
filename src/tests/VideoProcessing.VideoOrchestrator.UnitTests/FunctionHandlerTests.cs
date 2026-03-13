using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VideoProcessing.VideoOrchestrator.Application.UseCases;
using VideoProcessing.VideoOrchestrator.Domain.Events;
using VideoProcessing.VideoOrchestrator.Domain.Models;
using VideoProcessing.VideoOrchestrator.Lambda;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class FunctionHandlerTests
{
    private static string S3EventBodyWithOneRecord => JsonSerializer.Serialize(new S3ObjectCreatedEvent(Records:
    [
        new S3Record(S3: new S3Detail(
            Bucket: new S3Bucket("my-bucket"),
            Object: new S3Object("videos/usr-1/vid-1/original")))
    ]));

    [Fact]
    public async Task FunctionHandler_WhenBodyIsEmpty_ThrowsInvalidOperationException()
    {
        var (function, _) = CreateFunctionWithMocks();
        var sqsEvent = new SQSEvent
        {
            Records =
            [
                new SQSEvent.SQSMessage { MessageId = "msg-1", Body = "" }
            ]
        };
        var context = CreateLambdaContext();

        var act = () => function.FunctionHandler(sqsEvent, context);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Body*required*");
    }

    [Fact]
    public async Task FunctionHandler_WhenBodyIsNull_ThrowsInvalidOperationException()
    {
        var (function, _) = CreateFunctionWithMocks();
        var sqsEvent = new SQSEvent
        {
            Records =
            [
                new SQSEvent.SQSMessage { MessageId = "msg-1", Body = null }
            ]
        };
        var context = CreateLambdaContext();

        var act = () => function.FunctionHandler(sqsEvent, context);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FunctionHandler_WhenBodyIsInvalidJson_ThrowsJsonException()
    {
        var (function, _) = CreateFunctionWithMocks();
        var sqsEvent = new SQSEvent
        {
            Records =
            [
                new SQSEvent.SQSMessage { MessageId = "msg-1", Body = "{ invalid" }
            ]
        };
        var context = CreateLambdaContext();

        var act = () => function.FunctionHandler(sqsEvent, context);

        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task FunctionHandler_WhenRecordsNullOrEmpty_ProcessesWithoutCallingUseCases()
    {
        var (function, mocks) = CreateFunctionWithMocks();
        var sqsEvent = new SQSEvent
        {
            Records =
            [
                new SQSEvent.SQSMessage { MessageId = "msg-1", Body = "{}" },
                new SQSEvent.SQSMessage { MessageId = "msg-2", Body = """{"Records":[]}""" }
            ]
        };
        var context = CreateLambdaContext();

        await function.FunctionHandler(sqsEvent, context);

        mocks.FetchUseCase.Verify(
            x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mocks.OrchestrateUseCase.Verify(
            x => x.ExecuteAsync(It.IsAny<VideoDetails>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FunctionHandler_WhenRecordHasValidS3Event_CallsFetchAndOrchestrate()
    {
        var (function, mocks) = CreateFunctionWithMocks();
        var expectedDetails = new VideoDetails(
            "vid-1", "usr-1", "Title", "Uploaded",
            "videos/usr-1/vid-1/original", "my-bucket", "", "u@e.com", 0, 0, 1);
        mocks.FetchUseCase
            .Setup(x => x.ExecuteAsync("videos/usr-1/vid-1/original", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDetails with { S3Bucket = "" });
        mocks.OrchestrateUseCase
            .Setup(x => x.ExecuteAsync(It.IsAny<VideoDetails>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("arn:aws:states:us-east-1:123:execution:sm:run-1");

        var sqsEvent = new SQSEvent
        {
            Records =
            [
                new SQSEvent.SQSMessage { MessageId = "msg-1", Body = S3EventBodyWithOneRecord }
            ]
        };
        var context = CreateLambdaContext();

        await function.FunctionHandler(sqsEvent, context);

        mocks.FetchUseCase.Verify(
            x => x.ExecuteAsync("videos/usr-1/vid-1/original", It.IsAny<CancellationToken>()),
            Times.Once);
        mocks.OrchestrateUseCase.Verify(
            x => x.ExecuteAsync(
                It.Is<VideoDetails>(d => d.S3Bucket == "my-bucket" && d.VideoId == "vid-1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FunctionHandler_WhenS3KeyIsMissing_SkipsRecordAndDoesNotCallOrchestrate()
    {
        var body = JsonSerializer.Serialize(new S3ObjectCreatedEvent(Records:
        [
            new S3Record(S3: new S3Detail(
                Bucket: new S3Bucket("b"),
                Object: new S3Object(Key: null)))
        ]));
        var (function, mocks) = CreateFunctionWithMocks();
        var sqsEvent = new SQSEvent
        {
            Records = [new SQSEvent.SQSMessage { MessageId = "m1", Body = body }]
        };
        var context = CreateLambdaContext();

        await function.FunctionHandler(sqsEvent, context);

        mocks.FetchUseCase.Verify(
            x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mocks.OrchestrateUseCase.Verify(
            x => x.ExecuteAsync(It.IsAny<VideoDetails>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static (Function Function, (Mock<IFetchVideoDetailsUseCase> FetchUseCase, Mock<IOrchestrateVideoProcessingUseCase> OrchestrateUseCase) Mocks) CreateFunctionWithMocks()
    {
        var fetchMock = new Mock<IFetchVideoDetailsUseCase>();
        var orchestrateMock = new Mock<IOrchestrateVideoProcessingUseCase>();
        var services = new ServiceCollection();
        services.AddScoped(_ => fetchMock.Object);
        services.AddScoped(_ => orchestrateMock.Object);
        var provider = services.BuildServiceProvider(validateScopes: true);
        var function = new Function(provider);
        return (function, (fetchMock, orchestrateMock));
    }

    private static ILambdaContext CreateLambdaContext()
    {
        var logger = new Mock<ILambdaLogger>();
        var context = new Mock<ILambdaContext>();
        context.Setup(x => x.Logger).Returns(logger.Object);
        return context.Object;
    }
}
