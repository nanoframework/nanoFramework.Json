//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;

namespace nanoFramework.Json
{
    // TODO: Remove static
    /// <summary>
    /// Common settings for JSON converter.
    /// </summary>
    public static class JsonSerializerOptions
    {
        /// <summary>
        /// Gets or sets resolver which is used to find properties in target object when deserializing JSON.
        /// </summary>
        public static IMemberResolver Resolver { get; set; } = new MemberResolver();

        /// <summary>
        /// Determines whether a property's name uses a case-insensitive comparison during deserialization.
        /// The default value is false.
        /// </summary>
        /// <remarks>There is a performance cost associated when the value is true.</remarks>
        public static bool PropertyNameCaseInsensitive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether deserialization should throw exception when no property found.
        /// </summary>
        public static bool ThrowExceptionWhenPropertyNotFound { get; set; } = false;
    }
}
