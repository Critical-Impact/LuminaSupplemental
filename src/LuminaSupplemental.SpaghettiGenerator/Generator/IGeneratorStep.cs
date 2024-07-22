using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public interface IGeneratorStep
{
    public Type OutputType { get; }
    public string FileName { get; }
    
    public string Name { get; }

    public bool ShouldRun();

    public List<ICsv> Run();
}
