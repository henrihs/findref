using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using dnlib.DotNet;
using IModule = FindRef.Cli.Wrappers.IModule;

namespace FindRef.Cli
{
    public class ReferenceFinder : IDisposable
    {
        private readonly IFileIO _fileIo;
        private readonly IModuleLoader _loader;
        private ReferenceFinderOptions _options;
        private List<IModule> _modules;

        public ReferenceFinder(IFileIO fileIo, IModuleLoader loader, Action<ReferenceFinderOptions> optionsFunc)
        {
            _fileIo = fileIo;
            _loader = loader;
            _options = new ReferenceFinderOptions();
            optionsFunc.Invoke(_options);
        }

        public IEnumerable<(IFullName referee, IFullName reference)> FindReferences()
        {
            _modules = new List<IModule>();
            var dlls = _fileIo.GetFilePaths(_options.Directory, "*.dll", _options.SearchOption);
            foreach (var dll in dlls)
            {
                try
                {
                    _modules.Add(_loader.Load(dll));
                }
                catch (Exception exception)
                {
                    throw new LoadException(exception);
                    // _options.WriteVerbose($"Failed loading {dll} due to {exception.GetType()}");
                    // throw;
                }
            }

            return FindReferences(_modules, _options.FindReferenceName);
        }

        private IEnumerable<(IFullName referee, IFullName reference)> FindReferences(IEnumerable<IModule> modules, string findReferenceName)
        {
            foreach (var module in modules)
            {
                if (!HasReference(module, findReferenceName, out var fullReferenceNames))
                {
                    continue;
                }
                
                foreach (var fullReferenceName in fullReferenceNames)
                {
                    yield return (module, fullReferenceName);
                    // _options.Write($"+ {name} has a reference to {(_options.IsVerbose ? fullReferenceName : findReferenceName)}");
                }
                // else if (_options.IncludeUnmatched)
                // {
                //     // _options.Write($"- {name} has no references to {(findReferenceName)}");
                // }
            }
        }

        private bool HasReference(IModule module, string findReference, out IFullName[] fullNames)
        {
            fullNames = null;
            var refs = module.GetAssemblyRefs();
            if (_options.UseRegex)
            {
                FindByRegex(findReference, out fullNames, refs);
            }
            else
            {
                FindByExactMatch(findReference, out fullNames, refs);
            }

            return fullNames.FirstOrDefault() != null;
        }

        private static void FindByExactMatch(string findReference, out IFullName[] fullNames, IEnumerable<IFullName> refs)
        {
            fullNames = new[] { refs.FirstOrDefault(assembly => string.Equals(assembly.Name, findReference, StringComparison.OrdinalIgnoreCase)) };
        }

        private static void FindByRegex(string findReference, out IFullName[] fullNames, IEnumerable<IFullName> refs)
        {
            var regex = new Regex(findReference);
            fullNames = refs.Where(assembly => regex.IsMatch(assembly.Name)).Select(assembly => assembly).ToArray();
        }

        public void Dispose()
        {
            _modules.ForEach(m => m.Dispose());
        }
    }
}