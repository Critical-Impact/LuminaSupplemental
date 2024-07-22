using System;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

[Serializable]
public class GraphqlContainer<T>
{
    public T data { get; set; }
}
