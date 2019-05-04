using dnlib.DotNet;

namespace FindRef.Cli
{
    public class ModuleLoader : IModuleLoader
    {
        public ModuleDefMD Load(string filePath)
        {
            return ModuleDefMD.Load(filePath);
        }
    }

    public interface IModuleLoader
    {
        ModuleDefMD Load(string filePath);
    }
}