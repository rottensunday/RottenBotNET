namespace NETDiscordBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services;

    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder().Build().Run();

        private static IHostBuilder CreateHostBuilder()
            =>
                Host.CreateDefaultBuilder()
                    .ConfigureServices((_, services) =>
                    {
                        ConfigureServices(services);
                        services.AddHostedService<MainService>();
                    })
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        var builtConfig = config.Build();
                        
                        if (context.HostingEnvironment.IsProduction())
                        {
                            var secretClient = new SecretClient(
                                new Uri(builtConfig["KeyVaultUrl"]),
                                new DefaultAzureCredential());
                            
                            config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                        }
                        else if (context.HostingEnvironment.IsDevelopment())
                        {
                            using var store = new X509Store(StoreLocation.CurrentUser);
                            store.Open(OpenFlags.ReadOnly);
                            var certs = store.Certificates.Find(
                                X509FindType.FindByThumbprint,
                                builtConfig["AzureADCertThumbprint"], false);

                            config.AddAzureKeyVault(
                                new Uri(builtConfig["KeyVaultUrl"]),
                                new ClientCertificateCredential(
                                    builtConfig["AzureADDirectoryId"], 
                                    builtConfig["AzureADApplicationId"], 
                                    certs.OfType<X509Certificate2>().Single()),
                                new KeyVaultSecretManager());
                            store.Close();

                            // Overwrite Azure Key Vault Token entry when we already have token defined for DEV env
                            if (!string.IsNullOrEmpty(builtConfig["DiscordBotToken"]))
                            {
                                var initialData = new[]
                                {
                                    new KeyValuePair<string, string>("DiscordBotToken", builtConfig["DiscordBotToken"])
                                };

                                config.AddInMemoryCollection(initialData);
                            }
                        }
                    });
        
        private static void ConfigureServices(IServiceCollection collection)
        {
            collection
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<IDataAccessService, CosmosDbDataAccessService>();
        }
    }
}