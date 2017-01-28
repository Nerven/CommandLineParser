using System.Collections.Generic;

namespace Nerven.CommandLineParser
{
    public interface ICommandLineParser
    {
        IReadOnlyList<CommandLineItem> ParseCommandLine(CommandLine commandLine);
    }
}
