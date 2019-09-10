using dnlib.DotNet;
using FindRef.Cli.Wrappers;

namespace FindRef.Cli
{
    public class ModuleLoader : IModuleLoader
    {
        public Wrappers.IModule Load(string filePath)
        {
            return new ModuleDefWrapper(ModuleDefMD.Load(filePath));
        }
    }

    public interface IModuleLoader
    {
        Wrappers.IModule Load(string filePath);
    }
}