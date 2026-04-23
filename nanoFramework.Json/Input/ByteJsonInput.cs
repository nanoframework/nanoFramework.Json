//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.Text;

namespace nanoFramework.Json.Input
{
    internal sealed class ByteJsonInput : IJsonInput
    {
        private const char EndOfInput = (char)0xffff;

        private readonly byte[] _jsonBytes;
        private int _jsonPos;

        public ByteJsonInput(byte[] jsonBytes)
        {
            _jsonBytes = jsonBytes;
        }

        public char ReadChar()
        {
            return ReadUtf8CharFromBytes(_jsonBytes, ref _jsonPos);
        }

        public char PeekChar()
        {
            return _jsonPos >= _jsonBytes.Length ? EndOfInput : (char)_jsonBytes[_jsonPos];
        }

        public char ReadRawChar()
        {
            return _jsonPos >= _jsonBytes.Length ? EndOfInput : (char)_jsonBytes[_jsonPos++];
        }

        private static char ReadUtf8CharFromBytes(byte[] jsonBytes, ref int jsonPos)
        {
            if (jsonPos >= jsonBytes.Length)
            {
                return EndOfInput;
            }

            int charLength = GetUtf8CharLength(jsonBytes[jsonPos]);

            if (jsonPos + charLength > jsonBytes.Length)
            {
                return EndOfInput;
            }

            char ch = charLength == 1
                ? (char)jsonBytes[jsonPos]
                : Encoding.UTF8.GetChars(jsonBytes, jsonPos, charLength)[0];

            jsonPos += charLength;
            return ch;
        }

        private static int GetUtf8CharLength(byte value) =>
            (value & 0x80) == 0 ? 1
            : (value & 0x20) == 0 ? 2
            : (value & 0x10) == 0 ? 3
            : 4;
    }
}
