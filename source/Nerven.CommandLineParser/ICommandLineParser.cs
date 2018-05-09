namespace Nerven.CommandLineParser
{
    public interface ICommandLineParser
    {
        CommandLineItemCollection ParseCommandLine(CommandLine commandLine);
    }
}
