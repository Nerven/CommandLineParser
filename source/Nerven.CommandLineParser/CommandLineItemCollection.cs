using System.Collections;
using System.Collections.Generic;

namespace Nerven.CommandLineParser
{
    public sealed class CommandLineItemCollection : IReadOnlyList<CommandLineItem>
    {
        private readonly List<CommandLineItem> _Items;

        public CommandLineItemCollection(IReadOnlyCollection<CommandLineItem> items)
        {
            _Items = new List<CommandLineItem>(items.Count);
            _Items.AddRange(items);
        }

        public int Count => _Items.Count;

        public CommandLineItem this[int index] => _Items[index];

        public IEnumerator<CommandLineItem> GetEnumerator() => _Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
