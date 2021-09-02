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
        private bool _fOwnsContext;

        protected void EnterSerialization()
        {
            lock (JsonConvert.SyncObj)
            {
                if (JsonConvert.SerializationContext == null)
                {
                    JsonConvert.SerializationContext = new JsonConvert.SerializationCtx
                    {
                        Indent = 0
                    };

                    Monitor.Enter(JsonConvert.SerializationContext);

                    _fOwnsContext = true;
                }
            }
        }

        protected void ExitSerialization()
        {
            lock (JsonConvert.SyncObj)
            {
                if (_fOwnsContext)
                {
                    var monitorObj = JsonConvert.SerializationContext;
                    JsonConvert.SerializationContext = null;
                    _fOwnsContext = false;

                    Monitor.Exit(monitorObj);
                }
            }
        }

        protected void MarshallEName(string ename, byte[] buffer, ref int offset)
        {
            byte[] name = Encoding.UTF8.GetBytes(ename);

            if (buffer != null && ename.Length > 0)
            {
                Array.Copy(name, 0, buffer, offset, name.Length);
            }

            offset += name.Length;

            if (buffer != null)
            {
                buffer[offset] = 0;
            }

            ++offset;
        }

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
                if (buffer[current++] == 0)
                {
                    return current - 1;
                }
            }

            return -1;
        }
    }
}
