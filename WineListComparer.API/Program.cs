using WineListComparer.API;
using WineListComparer.API.Startup;
using WineListComparer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddCors(options => options.AddPolicy("AnyOrigin", o => o
    .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddInfrastructureServices();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<ImageResizeMiddleware>();

app.MapGet("/compare", async (IWineService wineService) =>
    {
        app.Logger.LogInformation("Running GET:Compare.");
        // await using var fileStream = new FileStream(@"C:\Temp\vinlista3.jpg", FileMode.Open);
        var result = await wineService.Process(new MemoryStream());

        return result;
    });

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

        await using var uploadStream = file.OpenReadStream();
        var result = await wineService.Process(uploadStream);

        return Results.Ok(result);
    })
    .Accepts<IFormFile>("multipart/form-data")
    .RequireCors("AnyOrigin");

app.Run();

