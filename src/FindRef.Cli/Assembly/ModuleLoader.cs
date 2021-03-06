using System;
using dnlib.DotNet;

namespace FindRef.Cli.Assembly
{
    public class ModuleLoader : IModuleLoader
    {
        public IModule Load(string filePath)
        {
            try
            {
                return new ModuleDefWrapper(ModuleDefMD.Load(filePath));
            }
            catch (Exception e)
            {
                return new FailedModule(filePath, e.Message);
            }
        }
    }

    public interface IModuleLoader
    {
        IModule Load(string filePath);
    }
}