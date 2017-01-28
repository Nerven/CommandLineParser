using System.Collections.Generic;
using Xunit;

namespace Nerven.CommandLineParser.Tests
{
    public class StandardCommandLineParserTests
    {
        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
                {
                    "command0",
                    new []
                        {
                            CommandLineItem.Command("command0"),
                        },
                };
            yield return new object[]
                {
                    string.Empty,
                    new CommandLineItem[]
                        {
                        },
                };
            yield return new object[]
                {
                    " ",
                    new CommandLineItem[]
                        {
                        },
                };
            yield return new object[]
                {
                    "command0 /opt1=opt1_val /flag2 --flag3 -4 argument5 \"--arg ument 6\" arg_7 -abcd=\"efgh i'h\"",
                    new []
                        {
                            CommandLineItem.Command("command0"),
                            CommandLineItem.Option("opt1", "opt1_val"),
                            CommandLineItem.Flag("flag2"),
                            CommandLineItem.Flag("flag3"),
                            CommandLineItem.Flag("4"),
                            CommandLineItem.Argument("argument5"),
                            CommandLineItem.Argument("--arg ument 6"),
                            CommandLineItem.Argument("arg_7"),
                            CommandLineItem.Flag("a"),
                            CommandLineItem.Flag("b"),
                            CommandLineItem.Flag("c"),
                            CommandLineItem.Option("d", "efgh i'h"),
                        },
                };
            yield return new object[]
                {
                    "\"co\"mman\"d0\" /\"opt1\"=\"opt1_val\" \"/flag2\" --flag3 -\"4\" arg\"ume\"nt5 \"--arg um\"ent 6\"\" arg_7 -\"abcd=\"\"efgh i'h\"",
                    new []
                        {
                            CommandLineItem.Command("command0"),
                            CommandLineItem.Option("opt1", "opt1_val"),
                            CommandLineItem.Argument("/flag2"),
                            CommandLineItem.Flag("flag3"),
                            CommandLineItem.Flag("4"),
                            CommandLineItem.Argument("argument5"),
                            CommandLineItem.Argument("--arg ument"),
                            CommandLineItem.Argument("6"),
                            CommandLineItem.Argument("arg_7"),
                            CommandLineItem.Flag("a"),
                            CommandLineItem.Flag("b"),
                            CommandLineItem.Flag("c"),
                            CommandLineItem.Option("d", "efgh i'h"),
                        },
                };
            yield return new object[]
                {
                    "--a asd",
                    new []
                        {
                            CommandLineItem.Flag("a"),
                            CommandLineItem.Command("asd"),
                        },
                };
            yield return new object[]
                {
                    "cmd -=a - = /",
                    new []
                        {
                            CommandLineItem.Command("cmd"),
                            CommandLineItem.Option(string.Empty, "a"),
                            CommandLineItem.Flag(string.Empty),
                            CommandLineItem.Argument("="),
                            CommandLineItem.Flag(string.Empty),
                        },
                };
            yield return new object[]
                {
                    "-=a - = /",
                    new []
                        {
                            CommandLineItem.Option(string.Empty, "a"),
                            CommandLineItem.Flag(string.Empty),
                            CommandLineItem.Command("="),
                            CommandLineItem.Flag(string.Empty),
                        },
                };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void ItCanDoSomethingWithContextKnowledge(string line, CommandLineItem[] expectedItems)
        {
            var _parser = StandardCommandLineParser.Default;
            var _splitter = CommandLineSplitter.Default;

            TestHelper.TestCommandLineParser(_parser, _splitter, line, expectedItems);
        }
    }
}
