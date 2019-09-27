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
            catch (BadImageFormatException)
            {
                Console.WriteLine($"Failed to load {filePath} due to BadImageFormatException");
                return new FailedModule(filePath);
            }
        }
    }

    public interface IModuleLoader
    {
        IModule Load(string filePath);
    }
}