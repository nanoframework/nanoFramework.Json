//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;

namespace nanoFramework.Json.Configuration
{
    /// <summary>
    /// Common settings for JSON converter.
    /// </summary>
    public static class Settings
    {
        public static IMemberResolver Resolver { get; set; } = new CaseSensitiveResolver();
    }
}
