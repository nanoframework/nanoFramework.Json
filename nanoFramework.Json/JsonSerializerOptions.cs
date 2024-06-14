//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

#nullable enable
using nanoFramework.Json.Resolvers;
using System;

namespace nanoFramework.Json
{
    /// <summary>
    /// Provides options to be used with <see cref="JsonSerializer"/>.
    /// </summary>
    public sealed class JsonSerializerOptions
    {
        private static JsonSerializerOptions? _defaultOptions;

        /// <summary>
        /// Constructs a new <see cref="JsonSerializerOptions"/> instance.
        /// </summary>
        public JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = false;
            Resolver = new MemberResolver();
            ThrowExceptionWhenPropertyNotFound = false;
        }

        /// <summary>
        /// Constructs a new <see cref="JsonSerializerOptions"/> instance with a predefined set of options determined by the specified <see cref="JsonSerializerDefaults"/>.
        /// </summary>
        /// <param name="defaults"> The <see cref="JsonSerializerDefaults"/> to reason about.</param>
        public JsonSerializerOptions(JsonSerializerDefaults defaults) : this()
        {
            if (defaults == JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true;
            }
            else if (defaults != JsonSerializerDefaults.General)
            {
                throw new ArgumentOutOfRangeException(nameof(defaults));
            }
        }

        // TODO: Find all test uses and use specific instances rather than changing defaults
        /// <summary>
        /// Gets a singleton instance of <see cref="JsonSerializerOptions" /> that uses the default configuration.
        /// </summary>
        public static JsonSerializerOptions Default
        {
            get
            {
                _defaultOptions ??= new JsonSerializerOptions();
                return _defaultOptions;
            }
        }

        /// <summary>
        /// Gets or sets resolver which is used to find properties in target object when deserializing JSON.
        /// </summary>
        public IMemberResolver Resolver { get; set; }

        /// <summary>
        /// Determines whether a property's name uses a case-insensitive comparison during deserialization.
        /// The default value is false.
        /// </summary>
        /// <remarks>There is a performance cost associated when the value is true.</remarks>
        public bool PropertyNameCaseInsensitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deserialization should throw exception when no property found.
        /// The default value is false.
        /// </summary>
        public bool ThrowExceptionWhenPropertyNotFound { get; set; }
    }
}
