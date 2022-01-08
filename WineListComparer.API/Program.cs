using WineListComparer.API.Startup;
using WineListComparer.Infra.Clients;
using WineListComparer.Infra.CognitiveServices;

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

app.MapGet("/compare", async (IWineParser parser) =>
    {
        // @"C:\Temp\vinlista1.jpg"
        var sentences = await OCRService.ReadImage(@"C:\Temp\vinlista3.jpg");
        var tasks = sentences.Select(x => parser.Parse(x));

        var hits = (await Task.WhenAll(tasks))
            .Where(x => x is not null);

        return hits;
    })
    .WithName("GetComparedWineList");

app.MapPost("/compare2", async (IWineParser parser, IFormFile file) =>
    {
        var sentences = await OCRService.ReadImage(@"C:\Temp\vinlista3.jpg");
        var tasks = sentences.Select(x => parser.Parse(x));

        var hits = (await Task.WhenAll(tasks))
            .Where(x => x is not null);

        return hits;
    })
    .WithName("GetComparedWineList2");

app.Run();

