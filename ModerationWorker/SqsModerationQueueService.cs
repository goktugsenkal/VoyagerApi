using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json.Linq;

namespace ModerationWorker;

public class SqsModerationQueueService
{
    private readonly IAmazonSQS _sqs;
    private readonly ILogger<SqsModerationQueueService> _logger;
    private readonly string _queueUrl;

    public SqsModerationQueueService(IAmazonSQS sqs, ILogger<SqsModerationQueueService> logger, IConfiguration config)
    {
        _sqs = sqs;
        _logger = logger;
        _queueUrl = config["AWS:SqsQueueUrl"]
                    ?? throw new InvalidOperationException("Missing SqsQueueUrl in config.");
    }

    public async Task<List<string>> ReceiveKeysAsync(CancellationToken cancellationToken)
    {
        var keys = new List<string>();

        try
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            };

            var response = await _sqs.ReceiveMessageAsync(request, cancellationToken);

            foreach (var message in response.Messages)
            {
                var key = ExtractS3Key(message.Body);

                if (!string.IsNullOrEmpty(key))
                {
                    keys.Add(key);
                    _logger.LogInformation("Queued moderation for: {Key}", key);
                }

                await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive from SQS.");
        }

        return keys;
    }

    private static string? ExtractS3Key(string body)
    {
        try
        {
            var json = JObject.Parse(body);
            return json["Records"]?[0]?["s3"]?["object"]?["key"]?.ToString();
        }
        catch
        {
            return null;
        }
    }
}