//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using nanoFramework.Json.Resolvers;

namespace nanoFramework.Json.Configuration
{
    /// <summary>
    /// Common settings for JSON converter. This class delegates to <see cref="JsonSerializerOptions"/>
    /// </summary>
    [Obsolete("Use JsonSerializerOptions instead")]
    public static class Settings
    {
        /// <summary>
        /// Gets or sets resolver which is used to find properties in target object when deserializing JSON.
        /// </summary>
        public static IMemberResolver Resolver
        {
            get => JsonSerializerOptions.Default.Resolver;
            set => JsonSerializerOptions.Default.Resolver = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether property resolving should be case-sensitive.
        /// </summary>
        public static bool CaseSensitive
        {
            get => !JsonSerializerOptions.Default.PropertyNameCaseInsensitive;
            set => JsonSerializerOptions.Default.PropertyNameCaseInsensitive = !value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether deserialization should throw exception when no property found.
        /// </summary>
        public static bool ThrowExceptionWhenPropertyNotFound
        {
            get => JsonSerializerOptions.Default.ThrowExceptionWhenPropertyNotFound;
            set => JsonSerializerOptions.Default.ThrowExceptionWhenPropertyNotFound = value;
        }
    }
}