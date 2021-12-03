//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json
{
    internal static class StringExtensions
    {
        internal static bool Contains(this string source, string search)
        {
            return source.IndexOf(search) >= 0;
        }

        internal static bool EndsWith(this string source, string search)
        {
            return source.IndexOf(search) == source.Length - search.Length;
        }
    }
}
