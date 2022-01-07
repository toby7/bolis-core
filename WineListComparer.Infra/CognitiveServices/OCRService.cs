using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace WineListComparer.Infra.CognitiveServices;

public static class OCRService
{
    private const string subscriptionKey = "6923a161f67b4696b20633c95e5de9a7";
    private const string endpoint = "https://bolis-ocr.cognitiveservices.azure.com/";

    public static async Task<string[]> ReadImage(string filePath)
    {
        var client = Authenticate(endpoint, subscriptionKey);
        // Extract text (OCR) from a URL image using the Read API
        return await ReadFile(client, filePath);
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

    private static async Task ReadFromFile()
    {
        var fileName = @"C:\Users\Jano\Documents\words.txt";

        //await using var fs = File.OpenRead(fileName);
        var file = new FileStream("path to file", FileMode.Open);

    }

    private static async Task<string[]> ReadFile(ComputerVisionClient client, string filePath)
    {
        // Read text from URL
        await using var file = new FileStream(filePath, FileMode.Open);
        //var analysis = await client.AnalyzeImageInStreamAsync(file, new List<VisualFeatureTypes?>() { VisualFeatureTypes.Brands });

        var textHeaders = await client.ReadInStreamAsync(file);

        //var textHeaders = await client.ReadAsync(urlFile);
        // After the request, get the operation location (operation ID)
        string operationLocation = textHeaders.OperationLocation;
        Thread.Sleep(2000);

        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        // Extract the text
        ReadOperationResult results;

        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while (results.Status is OperationStatusCodes.Running or OperationStatusCodes.NotStarted);

        // Display the found text.

        var sentencesList = new List<string>();
        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        foreach (var page in textUrlFileResults)
        {
            foreach (var line in page.Lines)
            {
                Console.WriteLine(line.Text);
                sentencesList.Add(line.Text);
            }
        }

        return sentencesList.ToArray();
    }
}

