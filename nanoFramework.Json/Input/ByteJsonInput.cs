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
        private char _pendingLowSurrogate;

        public ByteJsonInput(byte[] jsonBytes)
        {
            _jsonBytes = jsonBytes;
        }

        public char ReadChar()
        {
            return ReadUtf8CharFromBytes(advance: true);
        }

        public char PeekChar()
        {
            return ReadUtf8CharFromBytes(advance: false);
        }

        public char ReadRawChar()
        {
            return ReadUtf8CharFromBytes(advance: true);
        }

        private char ReadUtf8CharFromBytes(bool advance)
        {
            if (_pendingLowSurrogate != '\0')
            {
                char pending = _pendingLowSurrogate;

                if (advance)
                {
                    _pendingLowSurrogate = '\0';
                }

                return pending;
            }

            if (_jsonPos >= _jsonBytes.Length)
            {
                return EndOfInput;
            }

            int charLength = GetUtf8CharLength(_jsonBytes[_jsonPos]);

            if (_jsonPos + charLength > _jsonBytes.Length)
            {
                return EndOfInput;
            }

            char[] chars = charLength == 1
                ? null
                : Encoding.UTF8.GetChars(_jsonBytes, _jsonPos, charLength);

            if (advance)
            {
                _jsonPos += charLength;
            }

            if (charLength == 1)
            {
                return (char)_jsonBytes[_jsonPos - (advance ? 1 : 0)];
            }

            if (chars.Length == 2)
            {
                if (advance)
                {
                    _pendingLowSurrogate = chars[1];
                }

                return chars[0];
            }

            return chars[0];
        }

        private static int GetUtf8CharLength(byte value) =>
            (value & 0x80) == 0 ? 1
            : (value & 0x20) == 0 ? 2
            : (value & 0x10) == 0 ? 3
            : 4;
    }
}
