using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using McMaster.Extensions.CommandLineUtils;

namespace FindRef.Util
{
    public class CommandLineApp
    {
        private static bool _verbose;
        private static bool _useRegex;
        private static bool _includeUnmatched;

        public static int Main(string[] args)
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
            var optionRecurse = app.Option(
                "-r|--recursive", 
                "search directory recursively", 
                CommandOptionType.NoValue);
            var optionVerbose = app.Option(
                "-v|--verbose", 
                "write verbose output to stdout", 
                CommandOptionType.NoValue);
            var optionRegex = app.Option(
                "-e|--regex", 
                "use assemblyname argument as regex pattern", 
                CommandOptionType.NoValue);
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

                    var verbose = optionVerbose.HasValue();
                    var searchOption = optionRecurse.HasValue() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var directory = optionDirectory.HasValue() ? optionDirectory.Value() : Directory.GetCurrentDirectory();
                    var useRegex = optionRegex.HasValue();
                    var includeUnmatched = optionIncludeUnmatched.HasValue();
                    
                    var finder = new ReferenceFinder(Write, WriteVerbose, verbose, includeUnmatched, useRegex);
                    
                    var modules = finder.LoadModules(directory, searchOption).ToArray();

                    finder.FindReferences(modules, findReferenceName);
                    
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
    }
}