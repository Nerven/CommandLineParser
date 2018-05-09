using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

//// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Nerven.CommandLineParser
{
    public sealed class CustomCommandLineParser : ICommandLineParser
    {
        private readonly Func<Context, Func<CommandLinePart, CommandLineItem[]>> _CreateHandler;

        private CustomCommandLineParser(Func<Context, Func<CommandLinePart, CommandLineItem[]>> createHandler)
        {
            _CreateHandler = createHandler;
        }

        public static ICommandLineParser Create(Func<Context, Func<CommandLinePart, CommandLineItem[]>> createHandler)
        {
            if (createHandler == null)
            {
                throw new ArgumentNullException(nameof(createHandler));
            }

            return new CustomCommandLineParser(createHandler);
        }

        public CommandLineItemCollection ParseCommandLine(CommandLine commandLine)
        {
            if (commandLine.Parts == null)
            {
                throw new ArgumentException(nameof(commandLine));
            }

            var _items = new List<CommandLineItem>();
            var _handle = _CreateHandler(new Context(commandLine, new ReadOnlyCollection<CommandLineItem>(_items)));
            if (_handle == null)
            {
                throw new Exception("Handler may not be null.");
            }

            foreach (var _part in commandLine.Parts)
            {
                var _partItems = _handle(_part);
                if (_partItems != null)
                {
                    _items.AddRange(_partItems.Where(_item => _item != null));
                }
            }

            return new CommandLineItemCollection(_items);
        }

        public sealed class Context
        {
            public Context(CommandLine commandLine, IReadOnlyList<CommandLineItem> items)
            {
                CommandLine = commandLine;
                Items = items;
            }

            public CommandLine CommandLine { get; }

            public IReadOnlyList<CommandLineItem> Items { get; }
        }
    }
}
