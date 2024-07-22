using System;
using System.IO;
using System.Net;
using System.Net.Http;

using Autofac;

using Lumina;

using LuminaSupplemental.SpaghettiGenerator.Generator;

using Microsoft.Extensions.Configuration;
using SocksSharp;
using SocksSharp.Proxy;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class Program
    {
        static void Main()
        {
            var container = new Container();
            var builtContainer = container.Build();

            var generator = builtContainer.Resolve<Generator.Generator>();

            //TODO: Remove any static references
            Service.GameData = builtContainer.Resolve<GameData>();
            
            generator.Run();
        }
    }
}
