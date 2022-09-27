//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;
using System.Threading;

namespace nanoFramework.Json
{
    internal abstract class JsonToken
    {
        internal class SerializationCtx
        {
            public int Indent;
        }

        internal static SerializationCtx SerializationContext = null;

        public static string ConvertToString(byte[] byteArray, int start, int count)
        {
            var _chars = new char[byteArray.Length];

            Encoding.UTF8.GetDecoder().Convert(
                byteArray,
                start,
                count,
                _chars,
                0,
                byteArray.Length,
                false,
                out _,
                out int _charsUsed,
                out _);

            return new string(_chars, 0, _charsUsed);
        }

        public static int FindNul(byte[] buffer, int start)
        {
            int current = start;

            while (current < buffer.Length)
            {
                if (buffer[current++] != 0)
                {
                    continue;
                }

                return current - 1;
            }

            return -1;
        }
    }
}
