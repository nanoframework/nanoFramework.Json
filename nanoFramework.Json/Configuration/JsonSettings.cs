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
    public static class JsonSettings
    {
        /// <summary>
        /// Gets or sets resolver which is used to find properties in target object when deserializing JSON.
        /// </summary>
        public static IMemberResolver Resolver { get; set; } = new MemberResolver();

        /// <summary>
        /// Gets or sets a value indicating whether property resolving should be case sensitive.
        /// </summary>
        public static bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether deserialization should throw exception when no property found.
        /// </summary>
        public static bool ThrowExceptionWhenPropertyNotFound { get; set; } = false;
    }
}
