//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.IO;

namespace nanoFramework.Json.Input
{
    internal sealed class StreamReaderJsonInput : IJsonInput
    {
        private const char EndOfInput = (char)0xffff;

        private readonly StreamReader _reader;

        public StreamReaderJsonInput(StreamReader reader)
        {
            _reader = reader;
        }

        public char ReadChar()
        {
            int value = _reader.Read();
            return value < 0 ? EndOfInput : (char)value;
        }

        public char PeekChar()
        {
            int value = _reader.Peek();
            return value < 0 ? EndOfInput : (char)value;
        }

        public char ReadRawChar()
        {
            return ReadChar();
        }
    }
}
