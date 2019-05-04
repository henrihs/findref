using System.Collections.Generic;
using System.IO;

namespace FindRef.Cli
{
    public class FileIO : IFileIO
    {
        public IEnumerable<string> GetFilePaths(string directory, string searchpattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(directory, searchpattern, searchOption);
        }
    }
}