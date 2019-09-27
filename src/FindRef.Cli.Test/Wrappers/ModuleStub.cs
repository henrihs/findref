using System.Collections.Generic;
using dnlib.DotNet;
using IModule = FindRef.Cli.Assembly.IModule;

namespace FindRef.Cli.Test.Wrappers
{
    public class ModuleStub : Assembly.IModule
    {
        private readonly HashSet<IModule> _references = new HashSet<IModule>();

        public ModuleStub(string fullName, string name)
        {
            FullName = fullName;
            Name = name;
        }
        
        public void AddReference(IModule module)
        {
            _references.Add(module);
        }

        public IEnumerable<IFullName> GetAssemblyRefs()
        {
            return _references;
        }

        public string FullName { get; } 
        public UTF8String Name { get; set; }
        
        public void Dispose()
        {
            _references.Clear();
        }
    }
}