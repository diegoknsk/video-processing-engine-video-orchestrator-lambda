using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Domain.Exceptions;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

public sealed class ExternalServiceExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        const string message = "External API failed";

        var ex = new ExternalServiceException(message);

        ex.Message.Should().Be(message);
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        const string message = "Wrapped error";
        var inner = new InvalidOperationException("Inner");

        var ex = new ExternalServiceException(message, inner);

        ex.Message.Should().Be(message);
        ex.InnerException.Should().BeSameAs(inner);
    }
}
