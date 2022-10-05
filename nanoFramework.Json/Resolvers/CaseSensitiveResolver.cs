//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Reflection;

namespace nanoFramework.Json.Resolvers
{
    internal sealed class CaseSensitiveResolver : IMemberResolver
    {
        public MemberSet Get(string memberName, Type objectType)
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
                return GetInsensitive(memberName, objectType);
            }

            var memberPropSetMethod = objectType.GetMethod("set_" + memberName);
            if (memberPropSetMethod == null)
            {
                // failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.
                return GetInsensitive(memberName, objectType);
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType, false);
        }

        internal MemberSet GetInsensitive(string memberName, Type objectType)
        {
            var memberFieldInfo = GetFieldInfoCaseInsensitive(objectType, memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet(new SetValueDelegate((instance, value) => memberFieldInfo.SetValue(instance, value)), memberFieldInfo.FieldType, false);
            }

            var memberPropGetMethod = GetMethodCaseInsensitive(objectType, "get_" + memberName);
            if (memberPropGetMethod == null)
            {
                return new MemberSet(null, null, true);
            }

            var memberPropSetMethod = GetMethodCaseInsensitive(objectType, "set_" + memberName);
            if (memberPropSetMethod == null)
            {
                // failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.
                throw new DeserializationException();
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType, false);
        }

        private static FieldInfo GetFieldInfoCaseInsensitive(Type objectType, string fieldName)
        {
            foreach (var field in objectType.GetFields())
            {
                if (field.Name.ToLower() == fieldName.ToLower())
                {
                    return field;
                }
            }

            return null;
        }

        private static MethodInfo GetMethodCaseInsensitive(Type objectType, string methodName)
        {
            foreach (var method in objectType.GetMethods())
            {
                if (method.Name.ToLower() == methodName.ToLower())
                {
                    return method;
                }
            }

            return null;
        }
    }
}
