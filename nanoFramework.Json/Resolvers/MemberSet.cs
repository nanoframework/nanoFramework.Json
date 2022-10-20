//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Resolvers
{
    /// <summary>
    /// A delegate which will be used for setting value on object.
    /// </summary>
    /// <param name="objectInstance">Object instance which will be used to set value on.</param>
    /// <param name="valueToSet">Value which we want to set.</param>
    public delegate void SetValueDelegate(object objectInstance, object valueToSet);

    /// <summary>
    /// Data about member which we want to populate.
    /// </summary>
    public struct MemberSet
    {
        /// <summary>
        /// Gets a delegate which tells how to set a value on object.
        /// </summary>
        public SetValueDelegate SetValue { get; }

        /// <summary>
        /// Type of object we are trying to set value on.
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// Gets a value indicating whether current member should be skipped
        /// </summary>
        public bool Skip { get; }

        /// <summary>
        /// Initialize new instance of MemberSet struct.
        /// </summary>
        /// <param name="setValue">Deletage which tells how to set a value on object.</param>
        /// <param name="objectType">Type of object we are trying to set value on.</param>
        public MemberSet(SetValueDelegate setValue, Type objectType)
        {
            SetValue = setValue;
            ObjectType = objectType;
            Skip = false;
        }

        /// <summary>
        /// Initialize new instance of MemberSet struct.
        /// </summary>
        /// <param name="skip">Should skip current method.</param>
        public MemberSet(bool skip)
        {
            Skip = skip;
            ObjectType = null;
            SetValue = null;
        }
    }
}
