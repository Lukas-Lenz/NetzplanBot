using NetzplanBot;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NetzplanUnittest")]
/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/


string beisp = "A;Planung;3;-\n" +
            "B;Softwareentwicklung;7;A\n" +
            "C;Datenbankentwicklung;4;A\n" +
            "D;Testphase;1;B,C\n" +
            "E;Installation, Integration;2;D\n" +
            "F;Abnahme;1;E";

Netzplan np = new(beisp);