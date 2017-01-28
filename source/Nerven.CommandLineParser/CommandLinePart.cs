using System;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.CommandLineParser
{
    public struct CommandLinePart : IEquatable<CommandLinePart>, IEquatable<string>
    {
        internal CommandLinePart(int index, int originalStartIndex, string original, string value)
        {
            Index = index;
            OriginalStartIndex = originalStartIndex;
            Original = original;
            Value = value;
        }

        public int Index { get; }

        public int OriginalStartIndex { get; }

        public string Original { get; }

        public string Value { get; }

        public bool Equals(CommandLinePart other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public bool Equals(string other)
        {
            return string.Equals(Value, other, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is CommandLinePart)
            {
                return Equals((CommandLinePart)obj);
            }

            var _string = obj as string;
            if (_string != null)
            {
                return Equals(_string);
            }

            return false;
        }

        public override int GetHashCode() => Value?.GetHashCode() ?? string.Empty.GetHashCode();

        public override string ToString() => Value;
    }
}
