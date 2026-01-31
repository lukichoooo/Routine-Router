using System.ClientModel;
using Infrastructure.Configs;
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
            services.Configure<LLMConfig>(_ => config.GetSection("LLMConfig"));


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

            return services;
        }
    }
}
