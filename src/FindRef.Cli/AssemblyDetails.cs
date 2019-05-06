using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace FindRef.Cli
{
    public struct AssemblyDetails
    {
        public AssemblyDetails(string fullName, string version)
        {
            FullName = fullName;
            Version = version;
        }
    
        public AssemblyDetails(ModuleDef moduleDef)
        {
            var pattern = new Regex(@"(?<name>[^,]+), Version=(?<version>(\d+\.*)*)");
            var result = pattern.Match(moduleDef.FullName);
            FullName = result.Groups["name"].Value;
            Version = result.Groups["version"].Value;
        }
        
        public string FullName { get; }
        public string Version { get; }
    }
}