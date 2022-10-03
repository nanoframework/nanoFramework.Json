//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;
using System;
using System.Text;

namespace nanoFramework.Json.Configuration
{
    /// <summary>
    /// Common settings for JSON converter.
    /// </summary>
    public static class Settings
    {
        private static IMemberResolver resolver;
        private static bool caseSensitive = true;

        internal static IMemberResolver Resolver
        {
            get
            {
                if (resolver == null)
                {
                    resolver = CaseSensitive ? new CaseSensitiveResolver() : new CaseInsensitiveResolver();
                }

                return resolver;
            }

            set
            {
                resolver = value;
            }
        }

        /// <summary>
        /// Sets if the member property should be case sensitive.
        /// </summary>
        public static bool CaseSensitive
        {
            internal get
            {
                return caseSensitive;
            }

            set
            {
                if (caseSensitive != value)
                {
                    caseSensitive = value;
                    Resolver = null;
                }
            }
        }
    }
}
