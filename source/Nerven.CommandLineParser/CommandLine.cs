using System;
using System.Collections.Generic;
using System.Linq;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.CommandLineParser
{
    public struct CommandLine : IEquatable<CommandLine>, IEquatable<string>, IEquatable<string[]>
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
            return Parts == null
                ? other.Parts == null
                : other.Parts != null && Parts.SequenceEqual(other.Parts);
        }

        public bool Equals(string other)
        {
            return string.Equals(Value, other, StringComparison.Ordinal);
        }

        public bool Equals(string[] other)
        {
            if (Parts == null)
            {
                return other == null;
            }

            if (other == null || other.Length != Parts.Count)
            {
                return false;
            }

            for (var _i = 0; _i < other.Length; _i++)
            {
                if (!Parts[_i].Equals(other[_i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case CommandLine _line:
                    return Equals(_line);
                case string _string:
                    return Equals(_string);
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
