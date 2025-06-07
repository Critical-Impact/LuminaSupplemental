using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public abstract class GeneratorStep : IGeneratorStep
{
    public abstract Type OutputType { get; }

    public abstract string FileName { get; }

    public abstract string Name { get; }

    public virtual bool ShouldRun()
    {
        return true;
    }

    public abstract List<ICsv> Run(Dictionary<Type, List<ICsv>> stepData);

    public virtual List<Type>? PrerequisiteSteps => null;
}
