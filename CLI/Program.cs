using Application;
using CLI;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddApplication();
services.AddInfrastructure();

Controller.Run();
