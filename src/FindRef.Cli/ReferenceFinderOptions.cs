using System.IO;

namespace FindRef.Cli
{
    public class ReferenceFinderOptions
    {
        public bool UseRegex { get; set; }
        public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;
        public string Directory { get; set; } = ".";
        public string FindReferenceName { get; set; } = string.Empty;
    }
}