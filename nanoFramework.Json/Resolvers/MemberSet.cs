//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Resolvers
{
    public delegate void SetValueDelegate(object objectInstance, object valueToSet);

    public struct MemberSet
    {
        public SetValueDelegate SetValue { get; }
        public Type ObjectType { get; }
        public bool Skip { get; }

        public MemberSet(SetValueDelegate setValue, Type objectType, bool skip)
        {
            SetValue = setValue;
            ObjectType = objectType;
            Skip = skip;
        }
    }
}
