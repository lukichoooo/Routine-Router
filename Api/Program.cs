using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
