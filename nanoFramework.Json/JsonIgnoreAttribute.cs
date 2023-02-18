// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.

using System;

namespace nanoFramework.Json
{
    /// <summary>
    /// Hides properties from the json serializer
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        /// <summary>
        /// array of property names for json serializer to ignore
        /// </summary>
        public string[] PropertyNames { get; set; }

        /// <summary>
        /// Hides properties from the json serializer.
        /// </summary>
        /// <param name="getterNamesToIgnore">A comma separated list of property names to ignore in json.</param>
        public JsonIgnoreAttribute(string getterNamesToIgnore)
        {
            // Split by commas, then trim whitespace for each
            PropertyNames = getterNamesToIgnore.Split(',');
            for(int i = 0; i < PropertyNames.Length; i++)
            {
                PropertyNames[i] = PropertyNames[i].Trim();
            }
        }
    }
}
