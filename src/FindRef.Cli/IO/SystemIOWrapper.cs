using System.Collections.Generic;
using System.IO;

namespace FindRef.Cli.IO
{
    public class SystemIOWrapper : IFileIO
    {
        public IEnumerable<string> GetFilePaths(string directory, string searchpattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(directory, searchpattern, searchOption);
        }
    }
}