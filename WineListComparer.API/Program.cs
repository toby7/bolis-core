using WineListComparer.API.Startup;
using WineListComparer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddInfrastructureServices();
builder.Services.AddCors(options => options.AddPolicy("AnyOrigin", o => o.AllowAnyOrigin()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/compare", async (IWineService wineService, string test) =>
    {
        await using var fileStream = new FileStream(@"C:\Temp\vinlista3.jpg", FileMode.Open);
        var result = await wineService.ProcessWineList(fileStream as Stream);

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
        var file = form.Files["image"];

        if (file is null)
        {
            return Results.BadRequest("file is null");
        }

        await using var uploadStream = file.OpenReadStream(); // Move closer to usage?

        var result = await wineService.ProcessWineList(uploadStream);

         return Results.Ok(result.Wines);
    })
    .Accepts<IFormFile>("multipart/form-data")
    .RequireCors("AnyOrigin");

app.Run();

