using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace FindRef
{
    public static class Program
    {
        private static bool _verbose;
        private static bool _useRegex;
        private static bool _includeUnmatched;

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
            var optionRegex = app.Option("-e|--regex", "use assemblyname argument as regex pattern", CommandOptionType.NoValue);
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
                    _useRegex = optionRegex.HasValue();
                    _includeUnmatched = optionIncludeUnmatched.HasValue();
                    
                    var modules = LoadModules(directory, searchOption).ToArray();

                    FindReferences(modules, findReferenceName);
                    
                    foreach (var module in modules)
                    {
                        module.Dispose();
                    }
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

        private static void FindReferences(IEnumerable<ModuleDefMD> modules, string findReferenceName)
        {
            foreach (var module in modules)
            {
                var name = _verbose ? module.Assembly.FullName : module.Name.String;

                if (HasReference(module, findReferenceName, out var fullReferenceNames))
                {
                    foreach (var fullReferenceName in fullReferenceNames)
                    {
                        Write($"+ {name} has a reference to {(_verbose ? fullReferenceName : findReferenceName)}");
                    }
                }
                else if (_includeUnmatched)
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

        private static bool HasReference(ModuleDef module, string findReference, out string[] fullNames)
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
                fullNames = new[] { refs.FirstOrDefault(assembly => assembly.Name == findReference)?.FullName };
            }

            return fullNames != null;
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