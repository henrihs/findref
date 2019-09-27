using System.Collections.Generic;
using System.Linq;
using FindRef.Cli.IO;
using FindRef.Cli.Test.Wrappers;
using Xunit;

namespace FindRef.Cli.Test
{
    public class ResultWriterTests
    {
        [Fact]
        public void WriteMatch_NonVerbose_WritesOutput()
        {
            var output = new List<string>();
            var a = new ModuleStub(TestData.fullNameA, "A");
            var b = new ModuleStub(TestData.fullNameB, "B");
            var cut = new ResultWriter(s => output.Add(s));
            
            cut.WriteMatch((a,b), false);
            
            Assert.Equal("+ A has a reference to B", output.Single());
        }

        [Fact]
        public void WriteMatch_Verbose_WritesOutput()
        {
            var output = new List<string>();
            var a = new ModuleStub(TestData.fullNameA, "A");
            var b = new ModuleStub(TestData.fullNameB, "B");
            var cut = new ResultWriter(s => output.Add(s));
            
            cut.WriteMatch((a,b), true);
            
            Assert.Equal("+ A.FullName (42.0.0) has a reference to B.FullName (1.0.0)", output.Single());
        }
    }
}