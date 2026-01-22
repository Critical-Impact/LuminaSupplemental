using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

using Autofac;

using Lumina;
using Lumina.Excel;

using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.SpaghettiGenerator.Steps;
using LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using SocksSharp;
using SocksSharp.Proxy;

using ILogger = Serilog.ILogger;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public class Container
{
    private IContainer container;

    private ContainerBuilder containerBuilder;

    public Container()
    {
        var builder = new ContainerBuilder();
        var seriLog = new LoggerConfiguration()
                      .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose)
                      .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Verbose))
                      .CreateLogger();
        builder.RegisterInstance(seriLog).As<ILogger>().SingleInstance();
        builder.RegisterType<DataCacher>().SingleInstance();
        builder.RegisterType<Generator>().SingleInstance();
        builder.RegisterType<GeneratorOptions>().SingleInstance();
        builder.RegisterType<GTParser>().SingleInstance();
        builder.RegisterType<MappyParser>().SingleInstance();
        builder.RegisterType<LodestoneParser>().SingleInstance();
        builder.RegisterType<StoreParser>().SingleInstance();
        builder.RegisterType<GubalApi>().SingleInstance();
        builder.Register<AppConfig>(
            c =>
            {
                var configurationBuilder = new ConfigurationBuilder()
                                           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                           .AddIniFile("config.ini");
                var configuration = configurationBuilder.Build();

                // Create an instance of the configuration class and bind the values from the INI file
                var appConfig = new AppConfig();
                configuration.GetSection("Basic").Bind(appConfig.Basic);
                configuration.GetSection("Parsing").Bind(appConfig.Parsing);
                configuration.GetSection("Proxy").Bind(appConfig.Proxy);
                return appConfig;
            }).SingleInstance();
        builder.Register<HttpClient>(
            c =>
            {
                var appConfig = c.Resolve<AppConfig>();
                if( appConfig.Proxy.ProxyEnable )
                {
                    var settings = new ProxySettings()
                    {
                        Host = appConfig.Proxy.ProxyHost,
                        Port = appConfig.Proxy.ProxyPort,
                        Credentials = new NetworkCredential(  appConfig.Proxy.ProxyUsername, appConfig.Proxy.ProxyPassword )
                    };
                    var proxyClientHandler = new ProxyClientHandler< Socks5 >( settings );
                    return new HttpClient( proxyClientHandler );
                }
                return new HttpClient();
            }).SingleInstance();
        builder.Register<GameData>(
            c =>
            {
                var appConfig = c.Resolve<AppConfig>();
                return new GameData(appConfig.Basic.FFXIVGameDirectory, new LuminaOptions() { PanicOnSheetChecksumMismatch = false });
            }).SingleInstance();

        var dataAccess = Assembly.GetExecutingAssembly();

        builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => t.Name.EndsWith("Step"))
               .Where(c => c is { IsAbstract: false, IsInterface: false })
               .Where(c => c.IsAssignableTo(typeof(IGeneratorStep)))
               .As<IGeneratorStep>()
               .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => t.Name.EndsWith("Step"))
               .Where(c => c is { IsAbstract: false, IsInterface: false })
               .Where(c => c.IsAssignableTo(typeof(IDownloadStep)))
               .As<IDownloadStep>()
               .AsImplementedInterfaces();

        builder.RegisterGeneric((context, parameters) =>
        {
            var gameData = context.Resolve<GameData>();
            var method = typeof(GameData)
                         .GetMethods()
                         .FirstOrDefault(m => m.Name == "GetExcelSheet" && m.IsGenericMethod && m.GetParameters().Length == 2);
            if (method == null)
            {
                throw new InvalidOperationException("No matching GetExcelSheet method found.");
            }

            // Make the method generic with the specific type
            var genericMethod = method.MakeGenericMethod(parameters);
            return genericMethod.Invoke(gameData, [null, null])!;
        }).As(typeof(ExcelSheet<>));


        this.containerBuilder = builder;
    }

    public IContainer Build()
    {
        this.container = this.containerBuilder.Build();
        return this.container;
    }
}
