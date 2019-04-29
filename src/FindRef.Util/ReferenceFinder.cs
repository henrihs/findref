using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace FindRef.Util
{
    public class ReferenceFinder
    {
        private readonly bool _isVerbose;
        private readonly bool _includeUnmatched;
        private readonly bool _useRegex;
        private Action<string> Write { get; }
        private Action<string> WriteVerbose { get; }

        public ReferenceFinder(Action<string> write, Action<string> writeVerbose, bool isVerbose, bool includeUnmatched, bool useRegex)
        {
            _isVerbose = isVerbose;
            _includeUnmatched = includeUnmatched;
            _useRegex = useRegex;
            Write = write;
            WriteVerbose = writeVerbose;
        }
        
        public void FindReferences(IEnumerable<ModuleDefMD> modules, string findReferenceName)
        {
            foreach (var module in modules)
            {
                var name = _isVerbose ? module.Assembly.FullName : module.Name.String;

                if (HasReference(module, findReferenceName, out var fullReferenceNames))
                {
                    foreach (var fullReferenceName in fullReferenceNames)
                    {
                        Write($"+ {name} has a reference to {(_isVerbose ? fullReferenceName : findReferenceName)}");
                    }
                }
                else if (_includeUnmatched)
                {
                    Write($"- {name} has no references to {(findReferenceName)}");
                }
            }
        }

        public IEnumerable<ModuleDefMD> LoadModules(string directory, SearchOption searchOption)
        {
            WriteVerbose($"Loading DLLs from '{directory}'{(searchOption == SearchOption.AllDirectories ? " recursively" : string.Empty)}");
            var dlls = Directory.EnumerateFiles(directory, "*.dll", searchOption);
            var modules = LoadModules(dlls);
            Write(string.Empty);
            return modules;
        }

        private bool HasReference(ModuleDef module, string findReference, out string[] fullNames)
        {
            fullNames = null;
            var refs = module.GetAssemblyRefs();
            if (_useRegex)
            {
                var regex = new Regex(findReference);
                fullNames = refs.Where(assembly => regex.IsMatch(assembly.Name)).Select(assembly => assembly.FullName).ToArray();
            }
            else
            {
                fullNames = new[]
                {
                    refs.FirstOrDefault(assembly => string.Equals(assembly.Name, findReference, StringComparison.OrdinalIgnoreCase))?.FullName
                };
            }

            return !string.IsNullOrEmpty(fullNames.FirstOrDefault());
        }

        private IEnumerable<ModuleDefMD> LoadModules(IEnumerable<string> dlls)
        {
            var modules = new List<ModuleDefMD>();
            foreach (var dll in dlls)
            {
                try
                {
                    var module = ModuleDefMD.Load(dll);
                    modules.Add(module);
                }
                catch (Exception e)
                {
                    WriteVerbose($"Failed loading {dll} due to {e.GetType()}");
                }
            }
        
            return modules;
        }
    }
}