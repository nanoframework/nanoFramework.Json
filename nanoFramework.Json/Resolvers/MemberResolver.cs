//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using System;
using System.Reflection;

namespace nanoFramework.Json.Resolvers
{
    public sealed class MemberResolver : IMemberResolver
    {
        public MemberSet Get(string memberName, Type objectType)
        {
            var memberFieldInfo = objectType.GetField(memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet(new SetValueDelegate((instance, value) => memberFieldInfo.SetValue(instance, value)), memberFieldInfo.FieldType);
            }

            var memberPropGetMethod = objectType.GetMethod("get_" + memberName);
            if (memberPropGetMethod == null)
            {
                return HandleNullPropertyMember(memberName, objectType);
            }

            var memberPropSetMethod = objectType.GetMethod("set_" + memberName);
            if (memberPropSetMethod == null)
            {
                return HandleNullPropertyMember(memberName, objectType);
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType);
        }

        private MemberSet HandleNullPropertyMember(string memberName, Type objectType)
        {
            if (!Settings.CaseSensitive)
            {
                return GetInsensitive(memberName, objectType);
            }

            return HandlePropertyNotFound();
        }

        internal MemberSet GetInsensitive(string memberName, Type objectType)
        {
            var memberFieldInfo = GetFieldInfoCaseInsensitive(objectType, memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet(new SetValueDelegate((instance, value) => memberFieldInfo.SetValue(instance, value)), memberFieldInfo.FieldType);
            }

            var memberPropGetMethod = GetMethodCaseInsensitive(objectType, "get_" + memberName);
            if (memberPropGetMethod == null)
            {
                return HandlePropertyNotFound();
            }

            var memberPropSetMethod = GetMethodCaseInsensitive(objectType, "set_" + memberName);
            if (memberPropSetMethod == null)
            {
                return HandlePropertyNotFound();
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType);
        }

        private MemberSet HandlePropertyNotFound()
        {
            if (Settings.ThrowExceptionWhenPropertyNotFound)
            {
                throw new DeserializationException();
            }

            return new MemberSet(true);
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
