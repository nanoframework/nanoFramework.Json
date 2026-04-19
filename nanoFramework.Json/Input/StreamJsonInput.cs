//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.IO;
using System.Text;

namespace nanoFramework.Json.Input
{
    internal sealed class StreamJsonInput : IJsonInput
    {
        private const char EndOfInput = (char)0xffff;

        private readonly Stream _stream;
        private readonly byte[] _streamBuffer;
        private int _streamOffset;
        private int _streamCount;
        private bool _endOfStream;

        public StreamJsonInput(Stream stream, int bufferSize = 256)
        {
            _stream = stream;
            _streamBuffer = new byte[bufferSize < 4 ? 4 : bufferSize];
        }

        public char ReadChar()
        {
            if (!EnsureStreamBytes(1))
            {
                return EndOfInput;
            }

            int charLength = GetUtf8CharLength(_streamBuffer[_streamOffset]);

            if (!EnsureStreamBytes(charLength))
            {
                return EndOfInput;
            }

            char ch = charLength == 1
                ? (char)_streamBuffer[_streamOffset]
                : Encoding.UTF8.GetChars(_streamBuffer, _streamOffset, charLength)[0];

            _streamOffset += charLength;
            _streamCount -= charLength;

            return ch;
        }

        public char PeekChar()
        {
            return !EnsureStreamBytes(1) ? EndOfInput : (char)_streamBuffer[_streamOffset];
        }

        public char ReadRawChar()
        {
            if (!EnsureStreamBytes(1))
            {
                return EndOfInput;
            }

            char ch = (char)_streamBuffer[_streamOffset];
            _streamOffset++;
            _streamCount--;

            return ch;
        }

        private static int GetUtf8CharLength(byte value) =>
            (value & 0x80) == 0 ? 1
            : (value & 0x20) == 0 ? 2
            : (value & 0x10) == 0 ? 3
            : 4;

        private bool EnsureStreamBytes(int count)
        {
            if (_streamCount >= count)
            {
                return true;
            }

            if (_endOfStream)
            {
                return false;
            }

            if (_streamCount > 0 && _streamOffset > 0)
            {
                Array.Copy(_streamBuffer, _streamOffset, _streamBuffer, 0, _streamCount);
            }

            _streamOffset = 0;

            while (_streamCount < count)
            {
                int readCount = _stream.Read(_streamBuffer, _streamCount, _streamBuffer.Length - _streamCount);

                if (readCount <= 0)
                {
                    _endOfStream = true;
                    break;
                }

                _streamCount += readCount;
            }

            return _streamCount >= count;
        }
    }
}
