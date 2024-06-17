//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Resolvers
{
    /// <summary>
    /// Interface for property resolvers. 
    /// </summary>
    public interface IMemberResolver
    {
        /// <summary>
        /// Gets the data about member from object which we want to populate.
        /// </summary>
        /// <param name="memberName">Key from JSON property. Property name we are looking for.</param>
        /// <param name="objectType">Type of object in which <paramref name="memberName"/> should be.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> to be used.</param>
        /// <returns>Data about member which we want to populate based on passed parameters.</returns>
        MemberSet Get(string memberName, Type objectType, JsonSerializerOptions options);
    }
}