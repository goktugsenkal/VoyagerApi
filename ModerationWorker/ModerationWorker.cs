using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLaborsImage = SixLabors.ImageSharp.Image;

namespace ModerationWorker;

public class ModerationWorker : BackgroundService
{
    private readonly ILogger<ModerationWorker> _logger;
    private readonly IAmazonRekognition _rekognition;
    private readonly IAmazonS3 _s3;
    private readonly SqsModerationQueueService _queue;

    private const string BucketName = "voyager-s3-bucket";

    public ModerationWorker(ILogger<ModerationWorker> logger, IAmazonRekognition rekognition, IAmazonS3 s3, SqsModerationQueueService queue)
    {
        _logger = logger;
        _rekognition = rekognition;
        _s3 = s3;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
{
    var keys = await _queue.ReceiveKeysAsync(stoppingToken);

    foreach (var key in keys)
    {
        try
        {
            // ✅ Check for "moderated" tag first
            var tagResponse = await _s3.GetObjectTaggingAsync(new GetObjectTaggingRequest
            {
                BucketName = BucketName,
                Key = key
            }, stoppingToken);

            var alreadyModerated = tagResponse.Tagging
                .Any(t => t.Key == "moderated" && t.Value == "true");

            if (alreadyModerated)
            {
                _logger.LogInformation("Skipping already moderated image: {Key}", key);
                continue;
            }

            _logger.LogInformation("Moderating image {Key}.", key);

            var s3Object = await _s3.GetObjectAsync(BucketName, key, stoppingToken);
            await using var memoryStream = new MemoryStream();
            await s3Object.ResponseStream.CopyToAsync(memoryStream, stoppingToken);
            memoryStream.Position = 0;

            var request = new DetectModerationLabelsRequest
            {
                Image = new Amazon.Rekognition.Model.Image { Bytes = memoryStream },
                MinConfidence = 80
            };

            var result = await _rekognition.DetectModerationLabelsAsync(request, stoppingToken);

            var isViolation = result.ModerationLabels.Any(l =>
                new[] {
                    "Graphic Female Nudity", "Non-Explicit Nudity", "Explicit",
                    "Graphic Male Nudity", "Sexual Activity", "Sexual Situations",
                    "Sex Toys", "Adult Toys"
                }.Contains(l.Name) && l.Confidence >= 90);

            _logger.LogInformation("Labels for {Key}: {Labels}", key, string.Join(", ", result.ModerationLabels.Select(l => l.Name)));

            if (isViolation)
            {
                _logger.LogWarning("Image {Key} flagged for explicit content.", key);

                memoryStream.Position = 0;
                using var image = await SixLaborsImage.LoadAsync(memoryStream, stoppingToken);
                var font = SystemFonts.CreateFont("Arial", 48, FontStyle.Bold);
                var text = "X -️ Violation";

                var textOptions = new TextOptions(font)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Origin = new PointF(0, 0),
                    WrappingLength = image.Width
                };

                var textSize = TextMeasurer.MeasureSize(text, textOptions);
                float centerX = (image.Width - textSize.Width) / 2;
                float centerY = (image.Height - textSize.Height) / 2;

                image.Mutate(ctx =>
                {
                    ctx.GaussianBlur(25);
                    ctx.Pixelate(4); // doesn't work
                    ctx.DrawText(text, font, Color.Red, new PointF(centerX, centerY));
                });

                using var outputStream = new MemoryStream();
                await image.SaveAsJpegAsync(outputStream, stoppingToken);
                outputStream.Position = 0;

                await _s3.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = key,
                    InputStream = outputStream,
                    ContentType = "image/jpeg"
                }, stoppingToken);
            }

            // ✅ Tag the image as moderated
            await _s3.PutObjectTaggingAsync(new PutObjectTaggingRequest
            {
                BucketName = BucketName,
                Key = key,
                Tagging = new Tagging
                {
                    TagSet = new List<Tag>
                    {
                        new Tag { Key = "moderated", Value = "true" }
                    }
                }
            }, stoppingToken);

            _logger.LogInformation("Tagged image as moderated: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image {Key}", key);
        }
    }
}

    }
}
