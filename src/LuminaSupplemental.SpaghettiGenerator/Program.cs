using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

using Autofac;

using Lumina;

using LuminaSupplemental.SpaghettiGenerator.Generator;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Program
{
    static void Main(string[] args)
    {
        var runMode = ResolveRunMode(args);
        var commandLineArgs = new CommandLineArgs { RunMode = runMode };

        var container = new Container(commandLineArgs);
        var builtContainer = container.Build();

        if (!commandLineArgs.IsRunAll)
        {
            var steps = builtContainer.Resolve<IEnumerable<IGeneratorStep>>();
            var stepNames = steps.Select(s => s.GetType().Name).OrderBy(n => n).ToList();
            if (!stepNames.Any(n => string.Equals(n, runMode, StringComparison.OrdinalIgnoreCase)))
            {
                Console.Error.WriteLine($"Unknown step: '{runMode}'. Valid steps:");
                foreach (var name in stepNames)
                    Console.Error.WriteLine($"  {name}");
                Environment.Exit(1);
            }
        }

        Service.GameData = builtContainer.Resolve<GameData>();
        builtContainer.Resolve<Generator.Generator>().Run();
    }

    private static string ResolveRunMode(string[] args)
    {
        string runMode = null;

        var option = new Option<string?>("--run-mode");
        option.HelpName = "Generator step to run ('all', '*', or a step name)";
        option.Aliases.Add("-m");
        var root = new RootCommand("LuminaSupplemental data generator");
        root.Options.Add(option);
        root.SetAction(mode =>
        {
            runMode = mode.GetValue(option)!;
        });
        var exitCode = root.Parse(args).Invoke();
        if (exitCode != 0)
            Environment.Exit(exitCode);

        if (runMode != null)
            return runMode;

        runMode = Environment.GetEnvironmentVariable("RUN_MODE") ?? "";
        if (!string.IsNullOrWhiteSpace(runMode))
            return runMode;

        Console.Write("Enter run mode ('all', '*', or a step name): ");
        runMode = Console.ReadLine()?.Trim() ?? "";
        return string.IsNullOrWhiteSpace(runMode) ? "all" : runMode;
    }
}
}
