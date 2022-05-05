﻿using System;
using System.Threading.Tasks;
using CodeToData.Domain.Verbs.TypeReferences;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CodeToData;

internal static class Program
{
    private static IConfiguration s_configuration;
    private static IServiceCollection s_serviceCollection;
    private static IServiceProvider s_serviceProvider;

    private static void BuildConfiguration()
    {
        ConfigurationBuilder configuration = new();
        var environmentName = GetEnvironmentName();

        s_configuration = configuration.AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static void ConfigureServices()
    {
        s_serviceCollection = new ServiceCollection();

        // var appSettings = new AppSettings();
        // s_configuration.Bind("Settings", appSettings);

        s_serviceCollection.AddLogging(configure => configure.AddSerilog());

        #region Verbs

        s_serviceCollection
            .AddTransient<TypeReferencesVerb>()
            ;

        #endregion

        s_serviceProvider = s_serviceCollection.BuildServiceProvider();
    }

    private static string GetEnvironmentName()
    {
        var environmentName = GetRawEnvironmentName();
        return environmentName switch
        {
            "dev" => "development",
            "test" => "development",
            "uat" => "development",
            "prod" => "production",
            _ => "Local"
        };
    }

    private static string GetRawEnvironmentName()
    {
        var environmentName = Environment.GetEnvironmentVariable("env") ?? "Local";
        return environmentName.Trim().ToLower();
    }

    private static async Task Main(string[] args)
    {
        BuildConfiguration();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom
            .Configuration(s_configuration)
            .CreateLogger();
        ConfigureServices();

        Parser.Default
            .ParseArguments<TypeReferencesOptions>(args)
            .WithParsed<TypeReferencesOptions>(options =>
            {
                var verb = s_serviceProvider.GetService<TypeReferencesVerb>();
                verb?.Run(options).Wait();
            });
    }
}