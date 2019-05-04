using System;
using System.IO;

namespace FindRef.Cli
{
    public class ReferenceFinderOptions
    {
        public Action<string> Write { get; set; } = s => { };
        public Action<string> WriteVerbose { get; set; } = s => { };
        public bool IsVerbose { get; set; }
        public bool IncludeUnmatched { get; set; }
        public bool UseRegex { get; set; }
        public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
        public string Directory { get; set; } = ".";
        public string FindReferenceName { get; set; } = string.Empty;
    }
}