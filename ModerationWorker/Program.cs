using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Amazon.Rekognition;
using ModerationWorker;

var builder = Host.CreateApplicationBuilder(args);

// Get AWS credentials from config
var awsOptions = builder.Configuration.GetSection("AWS");
var accessKey = awsOptions["AccessKey"];
var secretKey = awsOptions["SecretKey"];
var region = awsOptions["Region"];
var credentials = new BasicAWSCredentials(accessKey, secretKey);
var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

var rekConfig = builder.Configuration.GetSection("Rekognition");
var rekCreds = new BasicAWSCredentials(rekConfig["AccessKey"], rekConfig["SecretKey"]);
var rekRegion = Amazon.RegionEndpoint.GetBySystemName(rekConfig["Region"]);



// Register AWS clients manually
builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(credentials, regionEndpoint));
builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(credentials, regionEndpoint));
builder.Services.AddSingleton<IAmazonRekognition>(_ => new AmazonRekognitionClient(rekCreds, rekRegion));

// Your worker + services
builder.Services.AddSingleton<SqsModerationQueueService>();
builder.Services.AddHostedService<ModerationWorker.ModerationWorker>();

var host = builder.Build();
host.Run();