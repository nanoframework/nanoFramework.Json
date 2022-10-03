//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Resolvers
{
    // TODO: Tests
    internal class CaseSensitiveResolver : IMemberResolver
    {
        public MemberSet GetResolver(string memberName, Type objectType)
        {
            var memberFieldInfo = objectType.GetField(memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet(new SetValueDelegate((instance, value) => memberFieldInfo.SetValue(instance, value)), memberFieldInfo.FieldType, false);
            }

            var memberPropGetMethod = objectType.GetMethod("get_" + memberName);
            if (memberPropGetMethod == null)
            {
                return new MemberSet(null, null, true);
            }

            var memberPropSetMethod = objectType.GetMethod("set_" + memberName);
            if (memberPropSetMethod == null)
            {
                // failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.
                throw new DeserializationException();
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType, false);
        }
    }
}
