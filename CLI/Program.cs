using Application;
using CLI;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddApplication();
services.AddInfrastructure();
services.AddSingleton<IController, ConsoleController>();
services.AddSingleton<App>();

var serviceProvider = services.BuildServiceProvider();

var app = serviceProvider.GetRequiredService<App>();

app.Run();
