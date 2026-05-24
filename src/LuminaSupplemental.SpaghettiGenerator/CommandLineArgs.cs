using System;

namespace LuminaSupplemental.SpaghettiGenerator;

public class CommandLineArgs
{
    public required string RunMode { get; init; }

    public bool IsRunAll => RunMode is "all" or "*";

    public bool MatchesStep(string stepName) =>
        IsRunAll || string.Equals(RunMode, stepName, StringComparison.OrdinalIgnoreCase);
}