using System;
using System.Collections.Generic;
using System.Linq;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.CommandLineParser
{
    public struct CommandLine : IEquatable<CommandLine>, IEquatable<string>
    {
        internal CommandLine(
            string original,
            string value,
            IReadOnlyList<CommandLinePart> parts)
        {
            Original = original;
            Value = value;
            Parts = parts;
        }

        public string Original { get; }

        public string Value { get; }

        public IReadOnlyList<CommandLinePart> Parts { get; }

        public string[] GetArgs() => Parts.Select(_part => _part.Value).ToArray();

        public bool Equals(CommandLine other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public bool Equals(string other)
        {
            return string.Equals(Value, other, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            var _string = obj as string;
            if (_string != null)
            {
                return Equals(_string);
            }

            if (obj is CommandLine)
            {
                return Equals((CommandLine)obj);
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? string.Empty.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
