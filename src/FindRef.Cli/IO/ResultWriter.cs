using System;
using dnlib.DotNet;
using FindRef.Cli.Assembly;

namespace FindRef.Cli.IO
{
    public class ResultWriter
    {
        private readonly Action<string> _writeAction;

        public ResultWriter(Action<string> writeAction)
        {
            _writeAction = writeAction;
        }
        
        public void WriteMatch((IFullName referee, IFullName reference) match, bool isVerbose)
        {
            if (!isVerbose)
            {
                _writeAction($"{match.referee.Name} has a reference to {match.reference.Name}");
                return;
            }
            
            var referee = new AssemblyDetails(match.referee);
            var reference = new AssemblyDetails(match.reference);
            _writeAction($"{referee.FullName} ({referee.Version}) has a reference to {reference.FullName} ({reference.Version})");
        }

        public void WriteFailed(FailedModule failedModule)
        {
            _writeAction($"Failed to load {failedModule.Name}: {failedModule.Reason}");
        }
    }
}