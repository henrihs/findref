using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace FindRef
{
    public static class Program
    {
        private static bool _verbose;

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();

            var argumentFindReference = app.Argument("assemblyname", "the name of the assembly to look for references to");

            var optionDirectory = app.Option(
                "-d|--directory <DIRECTORY>",
                "The root directory to search through (default: working directory)",
                CommandOptionType.SingleValue);
            var optionRecurse = app.Option("-r|--recursive", "search directory recursively", CommandOptionType.NoValue);
            var optionVerbose = app.Option("-v|--verbose", "write verbose output to stdout", CommandOptionType.NoValue);
            var optionIncludeUnmatched = app.Option(
                "-i|--include-unmatched",
                "include unmatched search results in the output",
                CommandOptionType.NoValue);

            app.OnExecute(
                () =>
                {
                    var findReferenceName = argumentFindReference.Value;
                    if (string.IsNullOrWhiteSpace(findReferenceName))
                    {
                        Write($"Specify which reference to find with the [{argumentFindReference.Name}] argument");
                        app.ShowHelp();
                        return;
                    }

                    _verbose = optionVerbose.HasValue();
                    var searchOption = optionRecurse.HasValue() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var directory = optionDirectory.HasValue() ? optionDirectory.Value() : Directory.GetCurrentDirectory();
                    var includeUnmatched = optionIncludeUnmatched.HasValue();
                    
                    var modules = LoadModules(directory, searchOption);

                    FindReferences(modules, findReferenceName, includeUnmatched);
                });

            return app.Execute(args);
        }

        private static void Write(string s)
        {
            Console.WriteLine(s);
        }

        private static void WriteVerbose(string s)
        {
            if (_verbose)
            {
                Write(s);
            }
        }

        private static void FindReferences(IEnumerable<ModuleDefMD> modules, string findReferenceName, bool includeUnmatched)
        {
            foreach (var module in modules)
            {
                var name = _verbose ? module.Assembly.FullName : module.Name.String;

                if (HasReference(module, findReferenceName, out var fullReferenceName))
                {
                    Write($"+ {name} has a reference to {(_verbose ? fullReferenceName : findReferenceName)}");
                }
                else if (includeUnmatched)
                {
                    Write($"- {name} has no references to {(findReferenceName)}");
                }
            }
        }

        private static IEnumerable<ModuleDefMD> LoadModules(string directory, SearchOption searchOption)
        {
            WriteVerbose($"Loading DLLs from '{directory}'{(searchOption == SearchOption.AllDirectories ? " recursively" : string.Empty)}");
            var dlls = Directory.EnumerateFiles(directory, "*.dll", searchOption);
            var modules = LoadModules(dlls);
            Write(string.Empty);
            return modules;
        }

        private static bool HasReference(ModuleDef module, string findReference, out string fullName)
        {
            var refs = module.GetAssemblyRefs();
            fullName = refs.FirstOrDefault(reference => reference.Name == findReference)?.FullName;
            return fullName != null;
        }

        private static IEnumerable<ModuleDefMD> LoadModules(IEnumerable<string> dlls)
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