using System;
using System.Collections.Generic;
using System.Linq;

namespace Nerven.CommandLineParser.Extensions
{
    public static class EnumerableCommandLineItemExtensions
    {
        private const StringComparison _DefaultStringComparison = StringComparison.Ordinal;

        public static IEnumerable<CommandLineItem> OfType(
            this IEnumerable<CommandLineItem> collection,
            CommandLineItemType itemType)
        {
            return collection
                .Where(_item => _item.Type == itemType);
        }

        public static IEnumerable<CommandLineItem> OfKey(
            this IEnumerable<CommandLineItem> collection,
            string key,
            StringComparison keyComparison = _DefaultStringComparison)
        {
            return collection
                .Where(_item => string.Equals(_item.Key, key, keyComparison));
        }

        public static CommandLineItem GetCommand(
            this IEnumerable<CommandLineItem> collection)
        {
            return collection
                .OfType(CommandLineItemType.Command)
                .Single();
        }

        public static CommandLineItem GetCommandOrDefault(
            this IEnumerable<CommandLineItem> collection)
        {
            return collection
                .OfType(CommandLineItemType.Command)
                .SingleOrDefault();
        }

        public static bool HasFlag(
            this IEnumerable<CommandLineItem> collection,
            string key,
            StringComparison keyComparison = _DefaultStringComparison)
        {
            return collection
                .OfType(CommandLineItemType.Flag)
                .OfKey(key, keyComparison)
                .Any();
        }

        public static IEnumerable<CommandLineItem> GetOptions(
            this IEnumerable<CommandLineItem> collection,
            string key,
            StringComparison keyComparison = _DefaultStringComparison)
        {
            return collection
                .OfType(CommandLineItemType.Option)
                .OfKey(key, keyComparison);
        }

        public static CommandLineItem GetOption(
            this IEnumerable<CommandLineItem> collection,
            string key,
            StringComparison keyComparison = _DefaultStringComparison)
        {
            return collection
                .GetOptions(key, keyComparison)
                .Single();
        }

        public static CommandLineItem GetOptionOrDefault(
            this IEnumerable<CommandLineItem> collection,
            string key,
            StringComparison keyComparison = _DefaultStringComparison)
        {
            return collection
                .GetOptions(key, keyComparison)
                .SingleOrDefault();
        }

        public static IEnumerable<CommandLineItem> GetArguments(
            this IEnumerable<CommandLineItem> collection)
        {
            return collection
                .OfType(CommandLineItemType.Argument);
        }

        public static CommandLineItem GetArgument(
            this IEnumerable<CommandLineItem> collection,
            int index)
        {
            return collection
                .OfType(CommandLineItemType.Argument)
                .Skip(index)
                .First();
        }

        public static CommandLineItem GetArgumentOrDefault(
            this IEnumerable<CommandLineItem> collection,
            int index)
        {
            return collection
                .OfType(CommandLineItemType.Argument)
                .Skip(index)
                .FirstOrDefault();
        }
    }
}
