using System.Collections.Generic;
using dnlib.DotNet;

namespace FindRef.Cli.Assembly
{
    public class FailedModule : IModule
    {
        public string FullName => Name;
        public UTF8String Name { get; set; }

        public FailedModule(string filename)
        {
            Name = filename;
        }
        
        public void Dispose()
        {
        }

        public IEnumerable<IFullName> GetAssemblyRefs()
        {
            return new IFullName[0];
        }
    }
}