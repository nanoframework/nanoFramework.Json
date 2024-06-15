using System;
using System.Text;

namespace nanoFramework.Json
{
    /// <summary>Specifies scenario-based default serialization options that can be used to construct a <see cref="T:nanoFramework.Json.JsonSerializerOptions" /> instance.</summary>
    public enum JsonSerializerDefaults
    {
        /// <summary>
        ///   <para>General-purpose option values. These are the same settings that are applied if a <see cref="T:nanoFramework.Json.JsonSerializerDefaults" /> member isn't specified.</para>
        ///   <para>For information about the default property values that are applied, see JsonSerializerOptions properties.</para>
        /// </summary>
        General,
        /// <summary>
        ///   <para>Option values appropriate to Web-based scenarios.</para>
        ///   <para>This member implies that:</para>
        ///   <para>- Property names are treated as case-insensitive.</para>
        /// </summary>
        Web,
    }
}
