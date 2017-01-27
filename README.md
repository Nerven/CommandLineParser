Command line parser/splitter in managed C#

````c#
var s = @"ffmpeg -i source.webm -vf ""setpts=2.0 * PTS"" target.webm";
var commandLine = CommandLineParser.Default.ParseString(s);
var args = commandLine.GetArgs(); // -> ["ffmpeg"", "-i", "source.webm", "-vf", "setpts=2.0 * PTS", "target.webm"]
````
