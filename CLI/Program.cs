using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddApplication();
services.AddInfrastructure();

var provider = services.BuildServiceProvider();

// call the entry point use case
// var boot = provider.GetRequiredService<BootRoutineUseCase>();

// await boot.Run();   // <-- Application is now running inside CLI
