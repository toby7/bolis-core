using Microsoft.AspNetCore.Mvc;
using WineListComparer.API.Startup;
using WineListComparer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddInfrastructureServices();
builder.Services.AddCors(options => options.AddPolicy("AnyOrigin", o => o.AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
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



        //httpRequest.
        //var uploads = "testpath";
        //await using var fileStream = File.OpenWrite(uploads);
        //await using var uploadStream = httpRequest.Body;
        //await uploadStream.CopyToAsync(fileStream);
        //return Results.Ok();

        var form = await httpRequest.ReadFormAsync();
        var file = form.Files["image"];

        if (file is null)
        {
            return Results.BadRequest("file is null");
        }

        await using var fileStream = File.OpenWrite(file.FileName);
        await using var uploadStream = file.OpenReadStream();
        await uploadStream.CopyToAsync(fileStream);

        return Results.Ok();

        var result = await wineService.ProcessWineList(httpRequest.Body);

        return Results.Ok(result.Wines);
    })
    .WithName("GetComparedWineList2")
    .Accepts<IFormFile>("multipart/form-data")
    .RequireCors("AnyOrigin");

app.Run();

