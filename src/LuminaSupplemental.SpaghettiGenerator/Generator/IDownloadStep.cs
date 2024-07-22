using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public interface IDownloadStep
{
    public string Name { get; }

    public bool ShouldRun();
    
    public void Run();
}
