using System;
using System.Collections.Generic;
using System.IO;
using FindRef.Cli.Assembly;
using FindRef.Cli.IO;
using McMaster.Extensions.CommandLineUtils;

namespace FindRef.Cli
{
    public static class CommandLineApp
    {
        private static bool _isVerbose;

        public static int Run(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();

            var argumentFindReference = app.Argument(
                "assemblyname",
                "the name of the assembly to look for references to. " +
                "Case insensitive, matches if the FullName of the referenced assembly is equal to the argument.");

            var optionDirectory = app.Option(
                "-d|--directory <DIRECTORY>",
                "the root directory to search through (default: working directory)",
                CommandOptionType.SingleValue);
            var optionRecurse = app.Option("-r|--recursive", "search directory recursively", CommandOptionType.NoValue);
            var optionVerbose = app.Option("-v|--verbose", "write verbose output to stdout", CommandOptionType.NoValue);
            var optionRegex = app.Option("-e|--regex", "use assemblyname argument as regex pattern", CommandOptionType.NoValue);

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

                    _isVerbose = optionVerbose.HasValue();
                    var searchOption = optionRecurse.HasValue() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var directory = optionDirectory.HasValue() ? optionDirectory.Value() : Directory.GetCurrentDirectory();
                    var useRegex = optionRegex.HasValue();

                    WriteVerbose($"Loading DLLs from '{directory}'{(searchOption == SearchOption.AllDirectories ? " recursively" : string.Empty)}");

                    using (var finder = new ReferenceFinder(
                        new SystemIOWrapper(),
                        new ModuleLoader(),
                        options =>
                        {
                            options.Directory = directory;
                            options.FindReferenceName = findReferenceName;
                            options.SearchOption = searchOption;
                            options.UseRegex = useRegex;
                        }))
                    {
                        var matches = finder.FindReferences(out var failedModules);
                    
                        var resultWriter = new ResultWriter(Write);

                        if (_isVerbose)
                        {
                            WriteFailures(failedModules, resultWriter);
                        }
                        
                        foreach (var match in matches)
                        {
                            resultWriter.WriteMatch(match, _isVerbose);
                        }
                    }
                });

            return app.Execute(args);
        }

        private static void WriteFailures(IEnumerable<FailedModule> failedModules, ResultWriter resultWriter)
        {
            foreach (var failedModule in failedModules)
            {
                resultWriter.WriteFailed(failedModule);
            }
        }

        private static void Write(string s)
        {
            Console.WriteLine(s);
        }

        private static void WriteVerbose(string s)
        {
            if (_isVerbose)
            {
                Write(s);
            }
        }
    }
}