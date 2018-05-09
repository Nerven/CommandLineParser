using System;
using System.Collections.Generic;
using System.Linq;

//// ReSharper disable UnusedMember.Global
namespace Nerven.CommandLineParser
{
    public sealed class CommandLineSplitter
    {
        private readonly char _DefaultSeparatorCharacter;
        private readonly char _DefaultQuoteCharacter;
        private readonly char _DefaultEscapeCharacter;
        private readonly char[] _SeparatorCharacters;
        private readonly char[] _QuoteCharacters;
        private readonly char[] _EscapeCharacters;

        private CommandLineSplitter(
            char defaultSeparatorCharacter,
            char defaultQuoteCharacter,
            char defaultEscapeCharacter,
            char[] separatorCharacters,
            char[] quoteCharacters,
            char[] escapeCharacters)
        {
            _DefaultSeparatorCharacter = defaultSeparatorCharacter;
            _DefaultQuoteCharacter = defaultQuoteCharacter;
            _DefaultEscapeCharacter = defaultEscapeCharacter;
            _SeparatorCharacters = separatorCharacters;
            _QuoteCharacters = quoteCharacters;
            _EscapeCharacters = escapeCharacters;
        }

        public static CommandLineSplitter Default { get; } = new CommandLineSplitter(
            ' ',
            '"',
            '\\',
            new[] { ' ', '\t' },
            new[] { '"', '\'' },
            new[] { '\\' });

        public static CommandLineSplitter WindowsCompatible { get; } = new CommandLineSplitter(
            ' ',
            '"',
            '\\',
            new[] { ' ', '\t' },
            new[] { '"' },
            new[] { '\\' });

        public static CommandLineSplitter Create(
            char defaultSeparatorCharacter,
            char defaultQuoteCharacter,
            char defaultEscapeCharacter,
            IEnumerable<char> separatorCharacters,
            IEnumerable<char> quoteCharacters,
            IEnumerable<char> escapeCharacters)
        {
            var _parser = new CommandLineSplitter(
                defaultSeparatorCharacter,
                defaultQuoteCharacter,
                defaultEscapeCharacter,
                separatorCharacters.ToArray(),
                quoteCharacters.ToArray(),
                escapeCharacters.ToArray());

            if (!_parser._IsSeparatorChar(defaultSeparatorCharacter))
            {
                throw new ArgumentException(nameof(defaultSeparatorCharacter));
            }

            if (!_parser._IsQuoteChar(defaultQuoteCharacter))
            {
                throw new ArgumentException(nameof(defaultQuoteCharacter));
            }

            if (_parser._SeparatorCharacters
                .Any(_separatorCharacter => _parser._IsQuoteChar(_separatorCharacter) || _parser._IsEscapeChar(_separatorCharacter)))
            {
                throw new ArgumentException(nameof(separatorCharacters));
            }

            if (_parser._QuoteCharacters
                .Any(_quoteCharacter => _parser._IsEscapeChar(_quoteCharacter)))
            {
                throw new ArgumentException(nameof(quoteCharacters));
            }

            return _parser;
        }

        public CommandLine ParseString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var _outputBuffer = new char[s.Length];
            var _outputBufferIndex = 0;
            var _partWindows = new List<Tuple<int, int, int, int>>();
            var _partStart = default(int?);
            var _partBufferStart = default(int?);
            var _quoteChar = default(char?);
            var _escapeChar = default(char?);
            var _escapedValueBuffer = new char[s.Length];

            for (var _i = 0; _i < s.Length; _i++)
            {
                var _c = s[_i];

                if (_partStart.HasValue)
                {
                    if (!_escapeChar.HasValue && _quoteChar.HasValue && _quoteChar == _c)
                    {
                        _quoteChar = null;
                    }
                    else
                    {
                        if (_escapeChar.HasValue)
                        {
                            if (!_quoteChar.HasValue && _IsSeparatorChar(_c))
                            {
                                _outputBuffer[_outputBufferIndex++] = _escapeChar.Value;
                                _partWindows.Add(Tuple.Create(_partStart.Value, _i, _partBufferStart.Value, _outputBufferIndex));
                                _partStart = null;
                            }
                            else if (_IsEscapeChar(_c) || _IsQuoteChar(_c))
                            {
                                _outputBuffer[_outputBufferIndex++] = _c;
                            }
                            else
                            {
                                _outputBuffer[_outputBufferIndex++] = _escapeChar.Value;
                                _outputBuffer[_outputBufferIndex++] = _c;
                            }

                            _escapeChar = null;
                        }
                        else if (_IsEscapeChar(_c))
                        {
                            _escapeChar = _c;
                        }
                        else if (!_quoteChar.HasValue && _IsQuoteChar(_c))
                        {
                            _quoteChar = _c;
                        }
                        else if (!_quoteChar.HasValue && _IsSeparatorChar(_c))
                        {
                            _partWindows.Add(Tuple.Create(_partStart.Value, _i, _partBufferStart.Value, _outputBufferIndex));
                            _partStart = null;
                        }
                        else
                        {
                            _outputBuffer[_outputBufferIndex++] = _c;
                        }
                    }
                }
                else
                {
                    if (!_IsSeparatorChar(_c))
                    {
                        _partStart = _i;
                        _partBufferStart = _outputBufferIndex;

                        if (_IsQuoteChar(_c))
                        {
                            _quoteChar = _c;
                        }
                        else if (_IsEscapeChar(_c))
                        {
                            _escapeChar = _c;
                        }
                        else
                        {
                            _outputBuffer[_outputBufferIndex++] = _c;
                        }
                    }
                }
            }

            if (_escapeChar.HasValue)
            {
                _outputBuffer[_outputBufferIndex++] = _escapeChar.Value;
            }

            if (_partStart.HasValue)
            {
                _partWindows.Add(Tuple.Create(_partStart.Value, s.Length, _partBufferStart.Value, _outputBufferIndex));
            }

            var _parts = new CommandLinePart[_partWindows.Count];
            var _partIndex = 0;
            foreach (var _partWindow in _partWindows)
            {
                var _start = _partWindow.Item1;
                var _length = _partWindow.Item2 - _start;
                var _bufferStart = _partWindow.Item3;
                var _bufferLength = _partWindow.Item4 - _bufferStart;
                var _original = s.Substring(_start, _length);
                var _value = new string(_outputBuffer, _bufferStart, _bufferLength);

                var _escapedValue = _GetEscapedValue(_value, _escapedValueBuffer);

                _parts[_partIndex] = new CommandLinePart(
                    _partIndex++,
                    _start,
                    _original,
                    _value,
                    _escapedValue);
            }
            
            return new CommandLine(
                s,
                string.Join(_DefaultSeparatorCharacter.ToString(), _parts.Select(_part => _part.EscapedValue)), 
                _parts);
        }

        private string _GetEscapedValue(string value, char[] escapedValueBuffer)
        {
            var _escapedValueBufferIndex = 0;
            var _containsSeparator = false;
            foreach (var _partChar in value)
            {
                if (_IsSeparatorChar(_partChar))
                {
                    _containsSeparator = true;
                    break;
                }
            }

            if (_containsSeparator)
                escapedValueBuffer[_escapedValueBufferIndex++] = _DefaultQuoteCharacter;

            foreach (var _partChar in value)
            {
                if (_containsSeparator ? _partChar == _DefaultQuoteCharacter : _IsQuoteChar(_partChar))
                {
                    escapedValueBuffer[_escapedValueBufferIndex++] = _DefaultEscapeCharacter;
                }

                escapedValueBuffer[_escapedValueBufferIndex++] = _partChar;
            }

            if (_containsSeparator)
                escapedValueBuffer[_escapedValueBufferIndex++] = _DefaultQuoteCharacter;

            return new string(escapedValueBuffer, 0, _escapedValueBufferIndex);
        }

        private bool _IsSeparatorChar(char c)
        {
            return _SeparatorCharacters.Contains(c);
        }

        private bool _IsQuoteChar(char c)
        {
            return _QuoteCharacters.Contains(c);
        }

        private bool _IsEscapeChar(char c)
        {
            return _EscapeCharacters.Contains(c);
        }

        public sealed class Builder
        {
            public Builder()
            {
                DefaultSeparatorCharacter = Default._DefaultSeparatorCharacter;
                DefaultQuoteCharacter = Default._DefaultQuoteCharacter;
                DefaultEscapeCharacter = Default._DefaultEscapeCharacter;
                SeparatorCharacters = new List<char>();
                QuoteCharacters = new List<char>();
                EscapeCharacters = new List<char>();
            }

            public char DefaultSeparatorCharacter { get; set; }

            public char DefaultQuoteCharacter { get; set; }

            public char DefaultEscapeCharacter { get; set; }

            public List<char> SeparatorCharacters { get; set; }

            public List<char> QuoteCharacters { get; set; }

            public List<char> EscapeCharacters { get; set; }

            public CommandLineSplitter Build()
            {
                return Create(
                    DefaultSeparatorCharacter,
                    DefaultQuoteCharacter,
                    DefaultEscapeCharacter,
                    SeparatorCharacters?.Count > 0 ? SeparatorCharacters : Default._SeparatorCharacters.ToList(),
                    QuoteCharacters?.Count > 0 ? QuoteCharacters : Default._QuoteCharacters.ToList(),
                    EscapeCharacters?.Count > 0 ? EscapeCharacters : Default._EscapeCharacters.ToList());
            }
        }
    }
}
