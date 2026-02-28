using System.ClientModel;
using Application.Interfaces.Command;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Configs;
using Infrastructure.EventPublishing;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Serializer;
using Infrastructure.Repos;
using Infrastructure.Services.Command;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace Infrastructure
{
    public static class InfrastructureConfig
    {
        public static IServiceCollection AddInfrastructure(
                this IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("infrastructure.json", optional: false, reloadOnChange: true)
                .Build();

            // settings
            services.Configure<LLMConfig>(config.GetSection("LLMConfig"));
            services.Configure<EventStoreConfig>(config.GetSection("EventStoreConfig"));


            // Logging
            services.AddLogging();


            //EF
            services.AddDbContext<EventContext>(options =>
                options.UseSqlite(config.GetConnectionString("EventContext")));
            services.AddDbContext<StateContext>(options =>
                options.UseSqlite(config.GetConnectionString("StateContext")));

            services.AddSingleton<IIdentityProvider, IdentityProvider>();

            // // Event Store Infrastructure
            services.AddSingleton<IJsonEventMapper, JsonEventMapper>();
            services.AddSingleton<IEventStore, SQLiteEventStore>();
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddSingleton<IUnitOfWork, SQLiteUnitOfWork>();

            // entity tracking
            services.AddSingleton<IEntityStateStore<ChecklistState, ChecklistId>, SQLiteChecklistStateStore>();
            services.AddSingleton<IEntityStateStore<UserState, UserId>, SQLiteUserStateStore>();

            // repos
            services.AddSingleton<IChecklistRepo, ChecklistRepo>();
            services.AddSingleton<IUserRepo, UserRepo>();

            // Services
            // TODO: create an interface for each llm service 
            // and register it
            services.AddSingleton<ChatClient>(
                    _ =>
                    {
                        var llmConfig = services.BuildServiceProvider()
                            .GetRequiredService<IOptions<LLMConfig>>()
                            .Value;

                        return new(
                            model: llmConfig.Model,
                            credential: new ApiKeyCredential(llmConfig.ApiKey),
                            options: new OpenAIClientOptions
                            {
                                Endpoint = new Uri(llmConfig.Uri)
                            });
                    }
            );

            // LLM
            services.AddSingleton<ICommandParser, CommandParser>();


            return services;
        }
    }
}
