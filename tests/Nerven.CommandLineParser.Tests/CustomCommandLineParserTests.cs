using System.Collections.Generic;
using Xunit;

namespace Nerven.CommandLineParser.Tests
{
    public class CustomCommandLineParserTests
    {
        public static IEnumerable<object[]> GetContextKnowledgeData()
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
                    "command0 /opt1 opt1_val /flag2 --flag3 -4 argument5 \"arg ument 6\" arg_7",
                    new []
                        {
                            CommandLineItem.Command("command0"),
                            CommandLineItem.Option("opt1", "opt1_val"),
                            CommandLineItem.Flag("flag2"),
                            CommandLineItem.Flag("flag3"),
                            CommandLineItem.Flag("4"),
                            CommandLineItem.Argument("argument5"),
                            CommandLineItem.Argument("arg ument 6"),
                            CommandLineItem.Argument("arg_7"),
                        },
                };
        }

        [Theory]
        [MemberData(nameof(GetContextKnowledgeData))]
        public void ItCanDoSomethingWithContextKnowledge(string line, CommandLineItem[] expectedItems)
        {
            var _parser = CustomCommandLineParser.Create(_context =>
                {
                    string _optionKey = null;
                    var _optionKeyIndex = -1;
                    return _part =>
                        {
                            if (_context.Items.Count == 0)
                            {
                                return CommandLineItem.Command(_part);
                            }

                            if (_part.Value.StartsWith("/opt"))
                            {
                                _optionKey = _part.Value.Substring(1);
                                _optionKeyIndex = _part.Index;
                                return null;
                            }

                            if (_part.Value.StartsWith("--opt"))
                            {
                                _optionKey = _part.Value.Substring(2);
                                _optionKeyIndex = _part.Index;
                                return null;
                            }

                            if (_part.Value.StartsWith("--"))
                            {
                                return CommandLineItem.Flag(_part.Value.Substring(2));
                            }

                            if (_part.Value.StartsWith("/") || _part.Value.StartsWith("-"))
                            {
                                return CommandLineItem.Flag(_part.Value.Substring(1));
                            }

                            if (_optionKey != null && _optionKeyIndex == _part.Index - 1)
                            {
                                return CommandLineItem.Option(_optionKey, _part.Value);
                            }

                            return CommandLineItem.Argument(_part);
                        };
                });
            var _splitter = CommandLineSplitter.Default;

            TestHelper.TestCommandLineParser(_parser, _splitter, line, expectedItems);
        }
    }
}
