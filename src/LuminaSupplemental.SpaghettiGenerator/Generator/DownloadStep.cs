using System;
using System.Collections.Generic;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Generator;

public abstract class DownloadStep : IDownloadStep
{
    public abstract string Name { get; }

    public virtual bool ShouldRun()
    {
        return true;
    }
    
    public abstract void Run();
}
