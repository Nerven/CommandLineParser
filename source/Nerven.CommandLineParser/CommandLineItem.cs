using System;

namespace Nerven.CommandLineParser
{
    public sealed class CommandLineItem : IEquatable<CommandLineItem>
    {
        private CommandLineItem(
            CommandLineItemType type,
            string id,
            string key,
            string value)
        {
            Type = type;
            Id = id;
            Key = key;
            Value = value;
        }

        public CommandLineItemType Type { get; }

        public string Id { get; }

        public string Key { get; }

        public string Value { get; }

        public static CommandLineItem Command(string key)
        {
            return new CommandLineItem(CommandLineItemType.Command, null, key, null);
        }

        public static CommandLineItem Command(CommandLinePart part)
        {
            return new CommandLineItem(CommandLineItemType.Command, null, part.Value, null);
        }

        public static CommandLineItem Flag(string key)
        {
            return new CommandLineItem(CommandLineItemType.Flag, null, key, null);
        }

        public static CommandLineItem Flag(CommandLinePart part)
        {
            return new CommandLineItem(CommandLineItemType.Flag, null, part.Value, null);
        }

        public static CommandLineItem Option(string key, string value)
        {
            return new CommandLineItem(CommandLineItemType.Option, null, key, value);
        }

        public static CommandLineItem Argument(string value)
        {
            return new CommandLineItem(CommandLineItemType.Argument, null, null, value);
        }

        public static CommandLineItem Argument(CommandLinePart part)
        {
            return new CommandLineItem(CommandLineItemType.Argument, null, null, part.Value);
        }

        public static CommandLineItem Other(string key, string value)
        {
            return new CommandLineItem(CommandLineItemType.Other, null, key, value);
        }

        public static implicit operator CommandLineItem[](CommandLineItem item)
        {
            return new[] { item };
        }

        public CommandLineItem WithId(string id)
        {
            return new CommandLineItem(Type, id, Key, Value);
        }

        public bool Equals(CommandLineItem other)
        {
            return other != null &&
                other.Type == Type &&
                string.Equals(other.Id, Id, StringComparison.Ordinal) &&
                string.Equals(other.Key, Key, StringComparison.Ordinal) &&
                string.Equals(other.Value, Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandLineItem);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        // Intended for debug and GetHashCode() use only, does no escaping
        public override string ToString()
        {
            switch (Type)
            {
                case CommandLineItemType.Command:
                    return $"{Key}";
                case CommandLineItemType.Flag:
                    return Key.Length == 1 ? $"-{Key}" : $"--{Key}";
                case CommandLineItemType.Option:
                    return Key.Length == 1 ? $"-{Key}={Value}" : $"--{Key}={Value}";
                case CommandLineItemType.Argument:
                    return $"{Value}";
                case CommandLineItemType.Other:
                    return string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value)
                        ? $"{Key}{Value}"
                        : $"{Key}={Value}";
                default:
                    return string.Empty;
            }
        }
    }
}
