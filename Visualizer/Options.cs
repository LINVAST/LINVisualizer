using CommandLine;

namespace LINVisualizer
{
    internal sealed class Options
    {
        [Option('v', "verbose", Default = false, Required = false, HelpText = "Verbose output")]
        public bool Verbose { get; set; }

        [Value(0, Required = true, HelpText = "Source path")]
        public string? Source { get; set; }
    }
}
