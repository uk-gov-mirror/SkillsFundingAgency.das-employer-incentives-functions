﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerIncentives.Infrastructure;
using SFA.DAS.EmployerIncentives.Infrastructure.Configuration;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(SFA.DAS.EmployerIncentives.Functions.LegalEntities.Startup))]
namespace SFA.DAS.EmployerIncentives.Functions.LegalEntities
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logBuilder =>
            {
                logBuilder.AddFilter(typeof(Startup).Namespace, LogLevel.Information); // this is because all logging is filtered out by defualt
                var rootDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".."));
                logBuilder.AddNLog(Path.Combine(rootDirectory, "nlog.config"));
            });

            var serviceProvider = builder.Services.BuildServiceProvider();
            
            var configuration = serviceProvider.GetService<IConfiguration>();

            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

#if DEBUG
            configBuilder.AddJsonFile("local.settings.json", optional: true);
#endif
            configBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

            var config = configBuilder.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));

            builder.Services.AddOptions();
            builder.Services.Configure<EmployerIncentivesApiOptions>(config.GetSection(EmployerIncentivesApiOptions.EmployerIncentivesApi));
            builder.Services.Configure<FunctionConfigurationOptions>(config.GetSection(FunctionConfigurationOptions.EmployerIncentivesFunctionsConfiguration));

            var logger = serviceProvider.GetService<ILoggerProvider>().CreateLogger(GetType().AssemblyQualifiedName);
            if (config["NServiceBusConnectionString"] == "UseDevelopmentStorage=true")
            {
                builder.Services.AddNServiceBus(logger, (options) =>
                {
                    options.EndpointConfiguration = (endpoint) =>
                    {
                        endpoint.UseTransport<LearningTransport>().StorageDirectory(Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")), @"src\SFA.DAS.EmployerIncentives.Functions.TestConsole\.learningtransport"));
                        return endpoint;
                    };
                });
            }
            else
            {
                builder.Services.AddNServiceBus(logger);
            }

            builder.Services
                .AddEmployerIncentivesService()
                ;
        }
    }
}
