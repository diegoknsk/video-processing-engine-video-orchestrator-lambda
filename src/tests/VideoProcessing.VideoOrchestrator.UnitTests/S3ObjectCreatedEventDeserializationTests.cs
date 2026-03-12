using System.Text.Json;
using FluentAssertions;
using VideoProcessing.VideoOrchestrator.Domain.Events;
using Xunit;

namespace VideoProcessing.VideoOrchestrator.UnitTests;

/// <summary>
/// Testa que o modelo Domain deserializa corretamente o JSON de evento S3 (Records[].s3.bucket.name e object.key).
/// </summary>
public sealed class S3ObjectCreatedEventDeserializationTests
{
    private static readonly string S3EventJson = """
        {
          "Records": [
            {
              "s3": {
                "bucket": { "name": "my-bucket" },
                "object": { "key": "videos/0468f438-1234-5678-abcd-111111111111/16c39167-1234-5678-abcd-222222222222/original" }
              }
            }
          ]
        }
        """;

    [Fact]
    public void Deserialize_ValidS3Event_PopulatesBucketNameAndKey()
    {
        var evt = JsonSerializer.Deserialize<S3ObjectCreatedEvent>(S3EventJson);

        evt.Should().NotBeNull();
        evt!.Records.Should().NotBeNull().And.HaveCount(1);
        var record = evt.Records![0];
        record.S3.Should().NotBeNull();
        record.S3!.Bucket!.Name.Should().Be("my-bucket");
        record.S3.Object!.Key.Should().Be("videos/0468f438-1234-5678-abcd-111111111111/16c39167-1234-5678-abcd-222222222222/original");
    }
}
