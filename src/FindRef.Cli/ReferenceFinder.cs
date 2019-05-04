using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace FindRef.Cli
{
    public class ReferenceFinder
    {
        private readonly IFileIO _fileIo;
        private readonly IModuleLoader _loader;
        private ReferenceFinderOptions _options;

        public ReferenceFinder(IFileIO fileIo, IModuleLoader loader, Action<ReferenceFinderOptions> optionsFunc)
        {
            _fileIo = fileIo;
            _loader = loader;
            _options = new ReferenceFinderOptions();
            optionsFunc.Invoke(_options);
        }

        public void FindReferences()
        {
            var modules = new List<ModuleDefMD>();
            var dlls = _fileIo.GetFilePaths(_options.Directory, "*.dll", _options.SearchOption);
            foreach (var dll in dlls)
            {
                try
                {
                    modules.Add(_loader.Load(dll));
                }
                catch (Exception exception)
                {
                    _options.WriteVerbose($"Failed loading {dll} due to {exception.GetType()}");
                    throw;
                }
            }

            FindReferences(modules, _options.FindReferenceName);

            foreach (var module in modules)
            {
                module.Dispose();
            }
        }

        private void FindReferences(IEnumerable<ModuleDefMD> modules, string findReferenceName)
        {
            foreach (var module in modules)
            {
                var name = _options.IsVerbose ? module.Assembly.FullName : module.Name.String;

                if (HasReference(module, findReferenceName, out var fullReferenceNames))
                {
                    foreach (var fullReferenceName in fullReferenceNames)
                    {
                        _options.Write($"+ {name} has a reference to {(_options.IsVerbose ? fullReferenceName : findReferenceName)}");
                    }
                }
                else if (_options.IncludeUnmatched)
                {
                    _options.Write($"- {name} has no references to {(findReferenceName)}");
                }
            }
        }

        private bool HasReference(ModuleDef module, string findReference, out string[] fullNames)
        {
            fullNames = null;
            var refs = module.GetAssemblyRefs();
            if (_options.UseRegex)
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
    }
}