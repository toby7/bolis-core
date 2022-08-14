using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using WineListComparer.Core.Extensions;

namespace WineListComparer.API;

public class ImageResizeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ImageResizeMiddleware> logger;

    public ImageResizeMiddleware(RequestDelegate next, ILogger<ImageResizeMiddleware> logger)
    {
        _next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.Request.HasFormContentType is false)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("error");
            return;
        }

        var form = await httpContext.Request.ReadFormAsync();
        var file = form.Files["image"];

        if (file is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("file is null.");
            return;
        }

        if (file.Length.ToMegabytes() < 4)
        {
            await _next(httpContext);
            return;
        }
        logger.LogInformation($"Received image with size {file.Length.ToMegabytes()} mb.");

        var resizedStream = new MemoryStream();
        await using var uploadStream = file.OpenReadStream();

        Func<Stream, Task> resize = async stream =>
        {
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);
            {

                image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
                await image.SaveAsync(resizedStream, new PngEncoder());
                resizedStream.Seek(0, SeekOrigin.Begin);
            }
        };

        await resize(uploadStream);

        while (resizedStream.Length.ToMegabytes() > 4)
        {
            var newStream = resizedStream;
            resizedStream = new MemoryStream();

            await resize(newStream);
        }

        logger.LogInformation($"Image size after resize: {resizedStream.Length.ToMegabytes()}");

        var formFileCollection = new FormFileCollection
        {
            new FormFile(resizedStream, 0, resizedStream.Length, "image", file.FileName)
        };
        httpContext.Request.Form = new FormCollection(null, formFileCollection);

        await _next(httpContext);
    }
}