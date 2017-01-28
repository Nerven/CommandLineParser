using System;
using System.Linq;
using Xunit;

namespace Nerven.CommandLineParser.Tests
{
    public static class TestHelper
    {
        public static void CommandLineIsSplittedLikeWin32Does(string line, CommandLineSplitter parser)
        {
            var _commandLine = parser.ParseString(line);
            var _actualArgs = _commandLine.GetArgs();
            var _expectedArgs = Win32CommandLineToArgvW.Split(line);

            Assert.True(_commandLine.Equals(_expectedArgs));
            Assert.False(_commandLine.Equals(_expectedArgs.Concat(new[] { string.Empty }).ToArray()));
            Assert.Equal(string.Join(Environment.NewLine, _expectedArgs), string.Join(Environment.NewLine, _actualArgs));
            Assert.Equal(_expectedArgs.Length, _commandLine.Parts.Count);
            for (var _i = 0; _i < _expectedArgs.Length; _i++)
            {
                Assert.Equal(_i, _commandLine.Parts[_i].Index);
                Assert.Equal(_expectedArgs[_i], _commandLine.Parts[_i].Value);
                Assert.True(_commandLine.Parts[_i].Equals(_expectedArgs[_i]));
            }
        }
    }
}
