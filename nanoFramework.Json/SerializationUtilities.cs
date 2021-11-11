//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace nanoFramework.Json
{
    internal static class SerializationUtilities
    {
        internal static void Marshall(byte[] buffer, ref int offset, object arg)
        {
            var type = arg.GetType();
            if (type == typeof(byte))
            {
                buffer[offset++] = (byte)arg;
            }
            else if (type == typeof(short))
            {
                buffer[offset++] = (byte)(((short)arg) & 0xff);
                buffer[offset++] = (byte)(((short)arg >> 8) & 0xff);
            }
            else if (type == typeof(ushort))
            {
                buffer[offset++] = (byte)(((ushort)arg) & 0xff);
                buffer[offset++] = (byte)(((ushort)arg >> 8) & 0xff);
            }
            else if (type == typeof(int))
            {
                buffer[offset++] = (byte)(((int)arg) & 0xff);
                buffer[offset++] = (byte)(((int)arg >> 8) & 0xff);
                buffer[offset++] = (byte)(((int)arg >> 16) & 0xff);
                buffer[offset++] = (byte)(((int)arg >> 24) & 0xff);
            }
            else if (type == typeof(uint))
            {
                buffer[offset++] = (byte)(((uint)arg) & 0xff);
                buffer[offset++] = (byte)(((uint)arg >> 8) & 0xff);
                buffer[offset++] = (byte)(((uint)arg >> 16) & 0xff);
                buffer[offset++] = (byte)(((uint)arg >> 24) & 0xff);
            }
            else if (type == typeof(long))
            {
                buffer[offset++] = (byte)(((long)arg) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 8) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 16) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 24) & 0xff);

                buffer[offset++] = (byte)(((long)arg >> 32) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 40) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 48) & 0xff);
                buffer[offset++] = (byte)(((long)arg >> 56) & 0xff);
            }
            else if (type == typeof(ulong))
            {
                buffer[offset++] = (byte)(((ulong)arg) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 8) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 16) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 24) & 0xff);

                buffer[offset++] = (byte)(((ulong)arg >> 32) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 40) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 48) & 0xff);
                buffer[offset++] = (byte)(((ulong)arg >> 56) & 0xff);
            }
            else if (type == typeof(DateTime))
            {
                Marshall(buffer, ref offset, ((DateTime)arg).Ticks);
            }
            else if (type == typeof(byte[])) // special case for the typecode array
            {
                var value = (byte[])arg;
                var length = value.Length;
                Array.Copy(value, 0, buffer, offset, length);
                offset += length;
            }
            else if (type == typeof(string)) // string and unknown types
            {
                var value = Encoding.UTF8.GetBytes(arg.ToString());
                int length = value.Length;
                Array.Copy(value, 0, buffer, offset, length);
                offset += length;
                buffer[offset++] = 0;
            }
            else if (type == typeof(float))
            {
                var bytes = BitConverter.GetBytes((double)((float)arg));
                foreach (var b in bytes)
                {
                    buffer[offset++] = b;
                }
            }
            else if (type == typeof(double))
            {
                var bytes = BitConverter.GetBytes((double)arg);
                foreach (var b in bytes)
                {
                    buffer[offset++] = b;
                }
            }
            else
            {
                // unsupported type for Marshall
                throw new SerializationException();
            }
        }

        internal static object Unmarshall(byte[] buffer, ref int offset, TypeCode tc)
        {
            object result;

            switch (tc)
            {
                case TypeCode.Empty: // secret code for the argument typecode array
                    var argCount = (int)buffer[offset++];
                    var tcArray = new TypeCode[argCount];

                    for (var i = 0; i < argCount; ++i)
                    {
                        tcArray[i] = (TypeCode)buffer[offset++];
                    }

                    result = tcArray;

                    break;

                case TypeCode.Byte:
                    result = buffer[offset++];
                    break;

                case TypeCode.Int16:
                    result = (short)(buffer[offset] | (buffer[offset + 1] << 8));
                    offset += 2;
                    break;

                case TypeCode.UInt16:
                    result = (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
                    offset += 2;
                    break;

                case TypeCode.Int32:
                    result = buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24);
                    offset += 4;
                    break;

                case TypeCode.UInt32:
                    result = (uint)(buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
                    offset += 4;
                    break;

                case TypeCode.Int64:
                    result = (long)(buffer[offset]
                             | (((ulong)buffer[offset + 1]) << 8)
                             | (((ulong)buffer[offset + 2]) << 16)
                             | (((ulong)buffer[offset + 3]) << 24)
                             | (((ulong)buffer[offset + 4]) << 32)
                             | (((ulong)buffer[offset + 5]) << 40)
                             | (((ulong)buffer[offset + 6]) << 48)
                             | (((ulong)buffer[offset + 7]) << 56));

                    offset += 8;

                    break;

                case TypeCode.UInt64:
                    result = buffer[offset]
                             | (((ulong)buffer[offset + 1]) << 8)
                             | (((ulong)buffer[offset + 2]) << 16)
                             | (((ulong)buffer[offset + 3]) << 24)
                             | (((ulong)buffer[offset + 4]) << 32)
                             | (((ulong)buffer[offset + 5]) << 40)
                             | (((ulong)buffer[offset + 6]) << 48)
                             | (((ulong)buffer[offset + 7]) << 56);

                    offset += 8;

                    break;

                case TypeCode.DateTime:
                    result = new DateTime((long)Unmarshall(buffer, ref offset, TypeCode.Int64));
                    break;

                case TypeCode.String:
                    var idxNul = JsonToken.FindNul(buffer, offset);

                    if (idxNul == -1)
                    {
                        // Missing ename terminator
                        throw new SerializationException();
                    }

                    result = JsonToken.ConvertToString(buffer, offset, idxNul - offset);
                    offset = idxNul + 1;

                    break;

                case TypeCode.Double:
                    var i64 = (long)(buffer[offset]
                                     | (((ulong)buffer[offset + 1]) << 8)
                                     | (((ulong)buffer[offset + 2]) << 16)
                                     | (((ulong)buffer[offset + 3]) << 24)
                                     | (((ulong)buffer[offset + 4]) << 32)
                                     | (((ulong)buffer[offset + 5]) << 40)
                                     | (((ulong)buffer[offset + 6]) << 48)
                                     | (((ulong)buffer[offset + 7]) << 56));

                    result = BitConverter.Int64BitsToDouble(i64);
                    offset += 8;

                    break;

                default:
                    // Unsupported type
                    throw new SerializationException();
            }

            return result;
        }
    }
}
