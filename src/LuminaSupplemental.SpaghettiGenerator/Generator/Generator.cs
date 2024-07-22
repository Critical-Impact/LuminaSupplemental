using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using CsvHelper;

using LuminaSupplemental.Excel.Model;

using Serilog;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public class Generator
{
    private readonly IEnumerable<IGeneratorStep> steps;
    private readonly IEnumerable<IDownloadStep> downloadSteps;
    private readonly GeneratorOptions generatorOptions;
    private readonly ILogger logger;

    public Generator(IEnumerable<IGeneratorStep> steps, IEnumerable<IDownloadStep> downloadSteps, GeneratorOptions generatorOptions, ILogger logger)
    {
        this.steps = steps;
        this.downloadSteps = downloadSteps;
        this.generatorOptions = generatorOptions;
        this.logger = logger;
    }

    public void Run()
    {
        DirectoryInfo info = new DirectoryInfo(this.generatorOptions.OutputPath);
        if (!info.Exists)
        {
            info.Create();
            this.logger.Information("Created output folder.");
        }

        foreach (var file in info.EnumerateFiles("*.csv"))
        {
            file.Delete();
        }
        this.logger.Information("Deleted existing CSV files.");

        
        this.logger.Information("Starting downloads.");
        foreach (var step in this.downloadSteps)
        {
            if (!step.ShouldRun())
            {
                this.logger.Information($"[{step.Name}] Skipping download step as condition was not met.");
                continue;
            }
            this.logger.Information($"[{step.Name}] Downloading new data");
            step.Run();
            this.logger.Information($"[{step.Name}] Finished downloading new data");
        }
        this.logger.Information("Starting generation.");
        foreach (var step in this.steps)
        {
            if (!step.ShouldRun())
            {
                this.logger.Information($"[{step.Name}] Skipping generation step as condition was not met.");
                continue;
            }
            this.logger.Information($"[{step.Name}] Generating data");
            var items = step.Run();
            this.logger.Information($"[{step.Name}] Finished generating data");
            var outputPath = Path.Combine(this.generatorOptions.OutputPath, step.FileName);
            this.logger.Information($"[{step.Name}] Writing to {outputPath}");
            this.WriteFile(step.OutputType, items, outputPath);
        }
        this.logger.Information("Generation complete.");
    }
    
    public void WriteFile(Type outputType, List< ICsv > items, string outputPath )
    {
        using var fileStream5 = new FileStream( outputPath, FileMode.Create );
        var writer = new CsvWriter( new StreamWriter( fileStream5 ), CultureInfo.CurrentCulture );
        writer.WriteHeader(outputType);
        writer.NextRecord();
        foreach( var item in items )
        {
            writer.WriteRecord(Convert.ChangeType(item, outputType));
            writer.NextRecord();
        }
        writer.Flush();
        fileStream5.Close();
    }

}
