using WineListComparer.API.Startup;
using WineListComparer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/compare", async (IWineService wineService) =>
    {
        // @"C:\Temp\vinlista1.jpg"
        await using var fileStream = new FileStream(@"C:\Temp\vinlista3.jpg", FileMode.Open);
        var result = await wineService.ProcessWineList(fileStream);

        return result;
    })
    .WithName("GetComparedWineList");

app.MapPost("/compare2", async (IWineService wineService, HttpRequest httpRequest) =>
    {
        if (httpRequest.HasFormContentType is false)
        {
            return Results.BadRequest();
        }

        var form = await httpRequest.ReadFormAsync();
        var file = form.Files["file"];

        if (file is null)
        {
            return Results.BadRequest();
        }
        
        await using var stream = file.OpenReadStream();
        
        var result = await wineService.ProcessWineList(stream);

        //var sentences = await OCRService.ReadImage(@"C:\Temp\vinlista3.jpg");
        //var tasks = sentences.Select(x => wineService.ProcessWineList(x));

        //var hits = (await Task.WhenAll(tasks))
        //    .Where(x => x is not null);

        return Results.Ok(result.Wines);
    })
    .WithName("GetComparedWineList2")
    .Accepts<IFormFile>("multipart/form-data");

app.Run();

