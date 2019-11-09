using System;
using System.IO;
using System.Linq;
using FindRef.Cli.Assembly;
using FindRef.Cli.IO;
using FindRef.Cli.Test.Wrappers;
using Moq;
using Xunit;

namespace FindRef.Cli.Test
{
    public class ReferenceFinderTests
    {
        [Fact]
        public void FindReferences_ExactMatch_ReturnsActualReferences()
        {
            const string ReferenceName = "A";
            const string RefereeName = "B";
            var a = new ModuleStub(TestData.fullNameA, ReferenceName);
            var b = new ModuleStub(TestData.fullNameB, RefereeName);
            b.AddReference(a);
            var cut = SetupCut(a, b,
                options =>
                {
                    options.Directory = ".";
                    options.FindReferenceName = "A";
                });

            var result = cut.FindReferences().ToArray();
            
            Assert.Single(result);
            Assert.Equal(RefereeName, result.Single().referee.Name);
            Assert.Equal(TestData.fullNameB, result.Single().referee.FullName);
            Assert.Equal(ReferenceName, result.Single().reference.Name);
            Assert.Equal(TestData.fullNameA, result.Single().reference.FullName);
        }

        [Fact]
        public void FindReferences_RegexMatch_ReturnsActualReferences()
        {
            const string ReferenceName = "Abcdef";
            const string RefereeName = "B";
            var a = new ModuleStub(TestData.fullNameA, ReferenceName);
            var b = new ModuleStub(TestData.fullNameB, RefereeName);
            b.AddReference(a);
            
            var cut = SetupCut(a, b,
                options =>
                {
                    options.Directory = ".";
                    options.FindReferenceName = ".*de.*";
                    options.UseRegex = true;
                });
            
            var result = cut.FindReferences().ToArray();
            
            Assert.Single(result);
            Assert.Equal(RefereeName, result.Single().referee.Name);
            Assert.Equal(TestData.fullNameB, result.Single().referee.FullName);
            Assert.Equal(ReferenceName, result.Single().reference.Name);
            Assert.Equal(TestData.fullNameA, result.Single().reference.FullName);
        }

        [Fact]
        public void FindReferences_SearchOptionsArePassed()
        {
            var fileIOMock = new Mock<IFileIO>();
            var moduleLoaderMock = new Mock<IModuleLoader>();
            var cut = new ReferenceFinder(fileIOMock.Object, moduleLoaderMock.Object,
                options => { options.SearchOption = SearchOption.AllDirectories; });

            cut.FindReferences();
            
            fileIOMock.Verify(f => f.GetFilePaths(It.IsAny<string>(), "*.dll", SearchOption.AllDirectories));
        }

        private static ReferenceFinder SetupCut(IModule moduleA, IModule moduleB, Action<ReferenceFinderOptions> options)
        {
            var fileIOMock = new Mock<IFileIO>();
            fileIOMock.Setup(f => f.GetFilePaths(".", "*.dll", It.IsAny<SearchOption>())).Returns(new[] { "A", "B" });
            var moduleLoaderMock = new Mock<IModuleLoader>();
            moduleLoaderMock.Setup(m => m.Load("A")).Returns(moduleA);
            moduleLoaderMock.Setup(m => m.Load("B")).Returns(moduleB);
            var cut = new ReferenceFinder(
                fileIOMock.Object,
                moduleLoaderMock.Object,
                options);
            return cut;
        }
    }
}