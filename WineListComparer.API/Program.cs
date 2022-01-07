using WineListComparer.Infra.Clients;
using WineListComparer.Infra;
using WineListComparer.Infra.CognitiveServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices();

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


//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//       new WeatherForecast
//       (
//           DateTime.Now.AddDays(index),
//           Random.Shared.Next(-20, 55),
//           summaries[Random.Shared.Next(summaries.Length)]
//       ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}