using System;
using System.Linq;

namespace Nerven.CommandLineParser
{
    public static class StandardCommandLineParser
    {
        public static ICommandLineParser Default { get; } = CustomCommandLineParser.Create(_CreateHandler);
        
        private static Func<CommandLinePart, CommandLineItem[]> _CreateHandler(CustomCommandLineParser.Context context)
        {
            return _part =>
                {
                    string _prefix;
                    if (_part.Original.StartsWith("--", StringComparison.Ordinal))
                    {
                        _prefix = "--";
                    }
                    else if (_part.Original.StartsWith("-", StringComparison.Ordinal))
                    {
                        _prefix = "-";
                    }
                    else if (_part.Original.StartsWith("/", StringComparison.Ordinal))
                    {
                        _prefix = "/";
                    }
                    else
                    {
                        if (context.Items.Any(_item => _item.Type == CommandLineItemType.Command))
                        {
                            return CommandLineItem.Argument(_part);
                        }

                        return CommandLineItem.Command(_part);
                    }

                    var _keyValue = _part.Value.Substring(_prefix.Length);
                    if (_keyValue.Length == 0)
                    {
                        return CommandLineItem.Flag(string.Empty);
                    }

                    var _valueStart = _keyValue.IndexOf('=');
                    if (_valueStart == -1)
                    {
                        if (_prefix == "-")
                        {
                            return Enumerable
                                .Range(0, _keyValue.Length)
                                .Select(_index => CommandLineItem.Flag(_keyValue[_index].ToString()))
                                .ToArray();
                        }

                        return CommandLineItem.Flag(_keyValue);
                    }

                    var _key = _keyValue.Substring(0, _valueStart);
                    var _value = _valueStart == _keyValue.Length - 1 ? string.Empty : _keyValue.Substring(_valueStart + 1);
                    if (_prefix == "-")
                    {
                        if (_key.Length == 0)
                        {
                            return CommandLineItem.Option(string.Empty, _value);
                        }

                        return Enumerable
                            .Range(0, _key.Length)
                            .Select(_index =>
                                {
                                    if (_index == _key.Length - 1)
                                    {
                                        return CommandLineItem.Option(_keyValue[_index].ToString(), _value);
                                    }

                                    return CommandLineItem.Flag(_keyValue[_index].ToString());
                                })
                            .ToArray();
                    }

                    return CommandLineItem.Option(_key, _value);
                };
        }
    }
}
