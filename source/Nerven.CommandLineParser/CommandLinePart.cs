using System;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.CommandLineParser
{
    public struct CommandLinePart : IEquatable<CommandLinePart>
    {
        internal CommandLinePart(string original, string value)
        {
            Original = original;
            Value = value;
        }

        public string Original { get; }

        public string Value { get; }

        public bool Equals(CommandLinePart other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is CommandLinePart)
            {
                return Equals((CommandLinePart)obj);
            }

            return false;
        }

        public override int GetHashCode() => Value?.GetHashCode() ?? string.Empty.GetHashCode();

        public override string ToString() => Value;
    }
}
