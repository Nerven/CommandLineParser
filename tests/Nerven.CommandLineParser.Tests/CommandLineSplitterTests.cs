using System;
using System.Linq;
using Xunit;

namespace Nerven.CommandLineParser.Tests
{
    public class CommandLineSplitterTests
    {
        private const string _ComplicatedCommandLineOriginal = "cmd\t" + @"/p1 /p2 --param3 -p4 ""param no 5"" bin\Param6.exe ""C:\Program Files\Param7\"""" ""I'm param 8'' and I'm still"" ""--AND-P9"" 'P a r a m ""10""' \'p11 p12\ ""Hi I'm""13"" cats …""!:> 14 \""p15";
        private const string _ComplicatedCommandLineParsed = @"cmd /p1 /p2 --param3 -p4 ""param no 5"" bin\Param6.exe ""C:\Program Files\Param7\"""" ""I'm param 8'' and I'm still"" --AND-P9 ""P a r a m \""10\"""" \'p11 p12\ ""Hi I'm13 cats …!:>"" 14 \""p15";
        private static readonly CommandLinePart[] _ComplicatedCommandLineParsedParts =
                {
                    new CommandLinePart(0, 0 ,"cmd", "cmd", "cmd"),
                    new CommandLinePart(1, 4, "/p1", "/p1", "/p1"),
                    new CommandLinePart(2, 8, "/p2", "/p2", "/p2"),
                    new CommandLinePart(3, 12, "--param3", "--param3", "--param3"),
                    new CommandLinePart(4, 21, "-p4", "-p4", "-p4"),
                    new CommandLinePart(5, 25, "\"param no 5\"", "param no 5", "\"param no 5\""),
                    new CommandLinePart(6, 38, "bin\\Param6.exe", "bin\\Param6.exe", "bin\\Param6.exe"),
                    new CommandLinePart(7, 53, "\"C:\\Program Files\\Param7\\\"\"", "C:\\Program Files\\Param7\"", "\"C:\\Program Files\\Param7\\\"\""),
                    new CommandLinePart(8, 81, "\"I'm param 8'' and I'm still\"", "I'm param 8'' and I'm still", "\"I'm param 8'' and I'm still\""),
                    new CommandLinePart(9, 111, "\"--AND-P9\"", "--AND-P9", "--AND-P9"),
                    new CommandLinePart(10, 122, "'P a r a m \"10\"'", "P a r a m \"10\"", "\"P a r a m \\\"10\\\"\""),
                    new CommandLinePart(11, 139, "\\'p11", "'p11", "\\'p11"),
                    new CommandLinePart(12, 145, "p12\\", "p12\\", "p12\\"),
                    new CommandLinePart(13, 150, "\"Hi I'm\"13\" cats …\"!:>", "Hi I'm13 cats …!:>", "\"Hi I'm13 cats …!:>\""),
                    new CommandLinePart(14, 173, "14", "14", "14"),
                    new CommandLinePart(15, 176, "\\\"p15", "\"p15", "\\\"p15"),
                };
        
        [Fact]
        public void ComplicatedCommandLineMakesCorrectCommandLineValue()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal);
            Assert.Equal(_ComplicatedCommandLineParsed, _commandLine.Value);
            Assert.False(_commandLine.Equals(_ComplicatedCommandLineOriginal));
            Assert.True(_commandLine.Equals(_ComplicatedCommandLineParsed));

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
            Assert.True(_commandLineParseFromValue.Equals(_ComplicatedCommandLineParsed));
        }

        [Fact]
        public void ComplicatedCommandLineMakesCorrectCommandLineParts()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal);
            var _expectedParts = _ComplicatedCommandLineParsedParts;

            Assert.Equal(
                string.Join(Environment.NewLine, _expectedParts.Select(_part => _part.Value)),
                string.Join(Environment.NewLine, _commandLine.Parts.Select(_part => _part.Value)));
            Assert.Equal(
                string.Join(Environment.NewLine, _expectedParts.Select(_part => _part.Original)),
                string.Join(Environment.NewLine, _commandLine.Parts.Select(_part => _part.Original)));
            Assert.Equal(_expectedParts.Length, _commandLine.Parts.Count);
            for (var _i = 0; _i < _expectedParts.Length; _i++)
            {
                Assert.Equal(_expectedParts[_i].Value, _commandLine.Parts[_i].Value);
                Assert.Equal(_expectedParts[_i].Original, _commandLine.Parts[_i].Original);
                Assert.Equal(_expectedParts[_i].EscapedValue, _commandLine.Parts[_i].EscapedValue);
                Assert.Equal(_expectedParts[_i], _commandLine.Parts[_i]);
                Assert.Equal(_i, _expectedParts[_i].Index);
                Assert.Equal(_i, _commandLine.Parts[_i].Index);
                Assert.Equal(_ComplicatedCommandLineOriginal.IndexOf(_commandLine.Parts[_i].Original, StringComparison.Ordinal), _commandLine.Parts[_i].OriginalStartIndex);
                Assert.Equal(_expectedParts[_i].OriginalStartIndex, _commandLine.Parts[_i].OriginalStartIndex);
            }
        }

        [Fact]
        public void ComplicatedCommandLineCanBeSlicedFromStartToEnd()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal).Slice(0, 16);

            Assert.Equal(_ComplicatedCommandLineParsed, _commandLine.Value);

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
        }

        [Fact]
        public void ComplicatedCommandLineCanBeSlicedToEnd()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal).Slice(1, 15);

            Assert.Equal(_ComplicatedCommandLineParsed.Substring(4), _commandLine.Value);

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
        }

        [Fact]
        public void ComplicatedCommandLineCanBeSlicedFromStart()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal).Slice(0, 12);

            Assert.Equal(_ComplicatedCommandLineParsed.Substring(0, 144), _commandLine.Value);

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
        }

        [Fact]
        public void ComplicatedCommandLineCanBeSlicedInTheMiddle()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_ComplicatedCommandLineOriginal).Slice(6, 3);

            Assert.Equal(_ComplicatedCommandLineParsed.Substring(38, 72), _commandLine.Value);

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
        }

        [Fact]
        public void ComplicatedCommandLineCanBeSlicedAndOriginalIsCorrectlyChanged()
        {
            var _parser = CommandLineSplitter.Default;
            var _commandLine = _parser.ParseString(_parser.ParseString(_ComplicatedCommandLineOriginal).Value).Slice(3, 9);

            Assert.Equal(_ComplicatedCommandLineParsed.Substring(12, 132), _commandLine.Value);

            var _commandLineParseFromValue = _parser.ParseString(_commandLine.Value);
            Assert.Equal(_commandLine.Value, _commandLineParseFromValue.Value);
            Assert.Equal(_commandLine.Original, _commandLineParseFromValue.Original);
        }

        [Fact]
        public void ComplicatedCommandLineIsSplittedLikeWin32DoesInWindowsCompatibilityMode()
        {
            TestHelper.CommandLineIsSplittedLikeWin32Does(_ComplicatedCommandLineOriginal, CommandLineSplitter.WindowsCompatible);
        }

        [Fact]
        public void ReadmeSampleIsCorrect()
        {
            var _expected = @"
var s = @""ffmpeg -i source.webm -vf """"setpts=2.0 * PTS"""" target.webm"";
var commandLine = CommandLineSplitter.Default.ParseString(s);
var args = commandLine.GetArgs(); // -> [""ffmpeg"", ""-i"", ""source.webm"", ""-vf"", ""setpts=2.0 * PTS"", ""target.webm""]";

            //// ReSharper disable InconsistentNaming
            var s = @"ffmpeg -i source.webm -vf ""setpts=2.0 * PTS"" target.webm";
            var commandLine = CommandLineSplitter.Default.ParseString(s);
            var args = commandLine.GetArgs();
            //// ReSharper restore InconsistentNaming

            var _actual = $@"
var {nameof(s)} = @""{s.Replace("\"", "\"\"")}"";
var {nameof(commandLine)} = {nameof(CommandLineSplitter)}.{nameof(CommandLineSplitter.Default)}.{nameof(CommandLineSplitter.ParseString)}({nameof(s)});
var {nameof(args)} = {nameof(commandLine)}.{nameof(CommandLine.GetArgs)}(); // -> [{string.Join(", ", args.Select(_arg => $"\"{_arg}\""))}]";

            Assert.Equal(_expected, _actual);
            Assert.Equal(commandLine.Value, CommandLineSplitter.Default.ParseString(commandLine.Value).Value);
        }

        [Theory]
        [InlineData(".")]
        ////[InlineData(@"ntbackup backup \\iggy-multi\c$ /m normal /j ""My Job 1"" /p ""Backup"" /n ""Command Line Backup 1"" /d ""Command Line Functionality"" /v:yes /r:no /l:s /rs:no /hc:on")]
        [InlineData("dir/Q/O:S d*")]
        [InlineData("diR /q d* /o:s")]
        [InlineData(@"ffmpeg -i source.webm -vf ""setpts=2.0*PTS"" target.webm")]
        ////[InlineData(@"winscp.com /command ""open sftp://martin@example.com/ -hostkey=""""ssh-rsa 2048 xx:xx:xx..."""""" ""exit""")]
        public void TypicalCommandLinesAreHandledLikeWindowsInBothDefaultModeAndCompatibilityMode(string line)
        {
            TestHelper.CommandLineIsSplittedLikeWin32Does(line, CommandLineSplitter.Default);
            TestHelper.CommandLineIsSplittedLikeWin32Does(line, CommandLineSplitter.WindowsCompatible);
        }

        [Theory]
        [InlineData(@"cvlc -vv --live-caching 2000 --decklink-audio-connection embedded --decklink-aspect-ratio 16:9 --decklink-mode hp50 decklink:// --sout-x264-preset fast --sout-x264-tune film --sout-transcode-threads 24 --no-sout-x264-interlaced --sout-x264-keyint 50 --sout-x264-lookahead 100 --sout-x264-vbv-maxrate 4000 --sout-x264-vbv-bufsize 4000 --sout '#duplicate{dst=""transcode{vcodec=h264,vb=6000,acodec=mp4a,aenc=fdkaac,ab=256}:std{access=udp,mux=ts,dst=192.168.1.2:4013}"", dst=""std{access=udp,mux=ts,dst=192.168.1.2:4014}""}'")]
        public void ApparentlyLessTypicalCommandLinesAreHandledLikeWindowsInCompatibilityMode(string line)
        {
            TestHelper.CommandLineIsSplittedLikeWin32Does(line, CommandLineSplitter.WindowsCompatible);
        }
    }
}
