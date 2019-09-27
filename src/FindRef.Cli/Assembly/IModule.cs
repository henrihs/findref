using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace FindRef.Cli.Assembly
{
    public interface IModule : IFullName, IDisposable
    {
        IEnumerable<IFullName> GetAssemblyRefs();
    }
}