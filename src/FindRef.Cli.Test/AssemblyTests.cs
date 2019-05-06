using dnlib.DotNet;
using Xunit;

namespace FindRef.Cli.Test
{
    public class AssemblyTests
    {
        [Fact]
        public void Ctor_ModuleDefInput_ParsesFullNameToNameAndVersionProps()
        {
            var name = "This.Is.A.FullName";
            var version = "3.2.0.0";
            var fullName = $"{name}, Version={version}, Culture=neutral, PublicKeyToken=50e96378b6e77783";

            var cut = new AssemblyDetails(new ModuleDefUser(fullName));
            
            Assert.Equal(name, cut.FullName);
            Assert.Equal(version, cut.Version);
        }
    }
}