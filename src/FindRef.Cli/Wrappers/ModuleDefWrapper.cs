using System.Collections.Generic;
using dnlib.DotNet;

namespace FindRef.Cli.Wrappers
{
    public class ModuleDefWrapper : IModule
    {
        private readonly ModuleDef _moduleDef;

        public ModuleDefWrapper(ModuleDef moduleDef)
        {
            _moduleDef = moduleDef;
        }

        public string FullName => _moduleDef.FullName;

        public UTF8String Name
        {
            get => _moduleDef.Name;
            set => _moduleDef.Name = value;
        }

        public IEnumerable<IFullName> GetAssemblyRefs()
        {
            return _moduleDef.GetAssemblyRefs();
        }

        public void Dispose()
        {
            _moduleDef?.Dispose();
        }
    }
}