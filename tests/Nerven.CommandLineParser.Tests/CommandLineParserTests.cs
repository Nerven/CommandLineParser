using System;
using System.Linq;
using Xunit;

namespace Nerven.CommandLineParser.Tests
{
    public class CommandLineParserTests
    {
        private const string _TestLineAOriginal = "cmd\t" + @"/p1 /p2 --param3 -p4 ""param no 5"" bin\Param6.exe ""C:\Program Files\Param7\"""" ""I'm param 8'' and I'm still"" ""--AND-P9"" 'P a r a m ""10""' \'p11 p12\ ""Hi I'm""13"" cats …""!:> 14 \""p15";
        private const string _TestLineAParsed = @"cmd /p1 /p2 --param3 -p4 ""param no 5"" bin\Param6.exe ""C:\Program Files\Param7"""" ""I'm param 8'' and I'm still"" --AND-P9 ""P a r a m ""10"""" 'p11 p12\ ""Hi I'm13 cats …!:>"" 14 ""p15";
        private static readonly CommandLinePart[] _TestLineAParsedParts =
                {
                    new CommandLinePart("cmd", "cmd"),
                    new CommandLinePart("/p1", "/p1"),
                    new CommandLinePart("/p2", "/p2"),
                    new CommandLinePart("--param3", "--param3"),
                    new CommandLinePart("-p4", "-p4"),
                    new CommandLinePart("\"param no 5\"", "param no 5"),
                    new CommandLinePart("bin\\Param6.exe", "bin\\Param6.exe"),
                    new CommandLinePart("\"C:\\Program Files\\Param7\\\"\"", "C:\\Program Files\\Param7\""),
                    new CommandLinePart("\"I'm param 8'' and I'm still\"", "I'm param 8'' and I'm still"),
                    new CommandLinePart("\"--AND-P9\"", "--AND-P9"),
                    new CommandLinePart("'P a r a m \"10\"'", "P a r a m \"10\""),
                    new CommandLinePart("\\'p11", "'p11"),
                    new CommandLinePart("p12\\", "p12\\"),
                    new CommandLinePart("\"Hi I'm\"13\" cats …\"!:>", "Hi I'm13 cats …!:>"),
                    new CommandLinePart("14", "14"),
                    new CommandLinePart("\\\"p15", "\"p15"),
                };

        [Fact]
        public void ComplicatedCommandLineMakesCorrectCommandLineValue()
        {
            var _parser = CommandLineParser.Default;
            var _commandLine = _parser.ParseString(_TestLineAOriginal);
            Assert.Equal(_TestLineAParsed, _commandLine.Value);
        }

        [Fact]
        public void ComplicatedCommandLineMakesCorrectCommandLineParts()
        {
            var _parser = CommandLineParser.Default;
            var _commandLine = _parser.ParseString(_TestLineAOriginal);
            var _expectedParts = _TestLineAParsedParts;

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
            }
        }

        [Fact]
        public void ComplicatedCommandLineIsSplittedLikeWin32DoesInWindowsCompatibilityMode()
        {
            _CommandLineIsSplittedLikeWin32Does(_TestLineAOriginal, CommandLineParser.WindowsCompatible);
        }

        [Fact]
        public void ReadmeSampleIsCorrect()
        {
            var _expected = @"var s = @""ffmpeg -i source.webm -vf """"setpts=2.0 * PTS"""" target.webm"";
var commandLine = CommandLineParser.Default.ParseString(s);
var args = commandLine.GetArgs(); // -> [""ffmpeg"", ""-i"", ""source.webm"", ""-vf"", ""setpts=2.0 * PTS"", ""target.webm""]";

            //// ReSharper disable InconsistentNaming
            var s = @"ffmpeg -i source.webm -vf ""setpts=2.0 * PTS"" target.webm";
            var commandLine = CommandLineParser.Default.ParseString(s);
            var args = commandLine.GetArgs();
            //// ReSharper restore InconsistentNaming

            var _actual = $@"var s = @""{s.Replace("\"", "\"\"")}"";
var commandLine = CommandLineParser.Default.ParseString(s);
var args = commandLine.GetArgs(); // -> [{string.Join(", ", args.Select(_arg => $"\"{_arg}\""))}]";

            Assert.Equal(_expected, _actual);
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
            _CommandLineIsSplittedLikeWin32Does(line, CommandLineParser.Default);
            _CommandLineIsSplittedLikeWin32Does(line, CommandLineParser.WindowsCompatible);
        }

        [Theory]
        [InlineData(@"cvlc -vv --live-caching 2000 --decklink-audio-connection embedded --decklink-aspect-ratio 16:9 --decklink-mode hp50 decklink:// --sout-x264-preset fast --sout-x264-tune film --sout-transcode-threads 24 --no-sout-x264-interlaced --sout-x264-keyint 50 --sout-x264-lookahead 100 --sout-x264-vbv-maxrate 4000 --sout-x264-vbv-bufsize 4000 --sout '#duplicate{dst=""transcode{vcodec=h264,vb=6000,acodec=mp4a,aenc=fdkaac,ab=256}:std{access=udp,mux=ts,dst=192.168.1.2:4013}"", dst=""std{access=udp,mux=ts,dst=192.168.1.2:4014}""}'")]
        public void ApparentlyLessTypicalCommandLinesAreHandledLikeWindowsInCompatibilityMode(string line)
        {
            _CommandLineIsSplittedLikeWin32Does(line, CommandLineParser.WindowsCompatible);
        }

        private static void _CommandLineIsSplittedLikeWin32Does(string line, CommandLineParser parser)
        {
            var _commandLine = parser.ParseString(line);
            var _actualArgs = _commandLine.GetArgs();
            var _expectedArgs = Win32CommandLineToArgvW.Split(line);

            Assert.Equal(string.Join(Environment.NewLine, _expectedArgs), string.Join(Environment.NewLine, _actualArgs));
            Assert.Equal(_expectedArgs.Length, _commandLine.Parts.Count);
            for (var _i = 0; _i < _expectedArgs.Length; _i++)
            {
                Assert.Equal(_expectedArgs[_i], _commandLine.Parts[_i].Value);
            }
        }
    }
}
