//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;
using System.Reflection;

namespace nanoFramework.Json.Resolvers
{
    internal sealed class MemberResolver : IMemberResolver
    {
        public MemberSet Get(string memberName, Type objectType, JsonSerializerOptions options)
        {
            var memberFieldInfo = objectType.GetField(memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet((instance, value) => memberFieldInfo.SetValue(instance, value), memberFieldInfo.FieldType);
            }

            var memberPropGetMethod = objectType.GetMethod("get_" + memberName);
            if (memberPropGetMethod is null)
            {
                return HandleNullPropertyMember(memberName, objectType, options);
            }

            var memberPropSetMethod = objectType.GetMethod("set_" + memberName);
            if (memberPropSetMethod is null)
            {
                return HandleNullPropertyMember(memberName, objectType, options);
            }

            return new MemberSet((instance, value) => memberPropSetMethod.Invoke(instance, new[] { value }), memberPropGetMethod.ReturnType);
        }

        private static FieldInfo GetFieldInfoCaseInsensitive(Type objectType, string fieldName)
        {
            foreach (var field in objectType.GetFields())
            {
                if (string.Equals(field.Name.ToLower(), fieldName.ToLower()))
                {
                    return field;
                }
            }

            return null;
        }

        internal MemberSet GetInsensitive(string memberName, Type objectType, JsonSerializerOptions options)
        {
            var memberFieldInfo = GetFieldInfoCaseInsensitive(objectType, memberName);

            // Value will be set via field
            if (memberFieldInfo is not null)
            {
                return new MemberSet((instance, value) => memberFieldInfo.SetValue(instance, value), memberFieldInfo.FieldType);
            }

            var memberPropGetMethod = GetMethodCaseInsensitive(objectType, "get_" + memberName);
            if (memberPropGetMethod is null)
            {
                return HandlePropertyNotFound(options);
            }

            var memberPropSetMethod = GetMethodCaseInsensitive(objectType, "set_" + memberName);
            if (memberPropSetMethod is null)
            {
                return HandlePropertyNotFound(options);
            }

            return new MemberSet((instance, value) => memberPropSetMethod.Invoke(instance, new[] { value }), memberPropGetMethod.ReturnType);
        }

        private static MethodInfo GetMethodCaseInsensitive(Type objectType, string methodName)
        {
            foreach (var method in objectType.GetMethods())
            {
                if (string.Equals(method.Name.ToLower(), methodName.ToLower()))
                {
                    return method;
                }
            }

            return null;
        }

        private MemberSet HandleNullPropertyMember(string memberName, Type objectType, JsonSerializerOptions options)
        {
            return options.PropertyNameCaseInsensitive ? GetInsensitive(memberName, objectType, options) : HandlePropertyNotFound(options);
        }

        private static MemberSet HandlePropertyNotFound(JsonSerializerOptions options)
        {
            if (options.ThrowExceptionWhenPropertyNotFound)
            {
                throw new DeserializationException();
            }

            return new MemberSet(true);
        }
    }
}
