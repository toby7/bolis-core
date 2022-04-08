using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;
using WineListComparer.Core.Services;
using WineListComparer.Core.Settings;

namespace WineListComparer.Infra.Services;

public sealed class OCRService : IOCRService
{
    private readonly OCRSettings settings;

    public OCRService(IOptions<OCRSettings> settings)
    {
        this.settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<string[]> ReadImage(Stream stream)
    {
        var client = Authenticate(settings.Endpoint, settings.Key);

        return await ReadFile(client, stream);
    }

    private static ComputerVisionClient Authenticate(string endpoint, string key)
    {
        var credentials = new ApiKeyServiceClientCredentials(key);

        var client = new ComputerVisionClient(credentials)
        {
            Endpoint = endpoint
        };

        return client;
    }

    private static async Task<string[]> ReadFile(ComputerVisionClient client, Stream stream)
    {
        //var analysis = await client.AnalyzeImageInStreamAsync(file, new List<VisualFeatureTypes?>() { VisualFeatureTypes.Brands });

        var textHeaders = await client.ReadInStreamAsync(stream);
        var operationLocation = textHeaders.OperationLocation;

        const int numberOfCharsInOperationId = 36;
        var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        ReadOperationResult results;

        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while (results.Status is OperationStatusCodes.Running or OperationStatusCodes.NotStarted);

        var sentencesList = new List<string>();
        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        foreach (var page in textUrlFileResults)
        {
            foreach (var line in page.Lines)
            {
                sentencesList.Add(line.Text);
            }
        }

        return sentencesList.ToArray();
    }
}

