using System.Collections.Generic;
using dnlib.DotNet;

namespace FindRef.Cli.Assembly
{
    public class FailedModule : IModule
    {
        public string FullName => Name;
        public UTF8String Name { get; set; }
        public string Reason { get; }

        public FailedModule(string filename, string reason)
        {
            Name = filename;
            Reason = reason;
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