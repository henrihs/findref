using System.Collections.Generic;
using System.IO;

namespace FindRef.Cli
{
    public interface IFileIO
    {
        IEnumerable<string> GetFilePaths(string directory, string searchpattern, SearchOption searchOption);
    }
}