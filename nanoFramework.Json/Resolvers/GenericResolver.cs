using System;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    internal class GenericResolver : IMemberResolver
    {
        public MemberSet GetResolver(string memberName, Type objectType)
        {
            // return from static methods (singletons)
            var memberFieldInfo = objectType.GetField(memberName);

            // Value will be set via field
            if (memberFieldInfo != null)
            {
                return new MemberSet(new SetValueDelegate((instance, value) => memberFieldInfo.SetValue(instance, value)), memberFieldInfo.FieldType);
            }

            var memberPropGetMethod = objectType.GetMethod("get_" + memberName);
            var memberPropSetMethod = objectType.GetMethod("set_" + memberName);

            if (memberPropSetMethod == null || memberPropGetMethod == null)
            {
                // failed to get setter of memberType {rootType.Name}. Possibly this property doesn't have a setter.
                throw new DeserializationException();
            }

            return new MemberSet(new SetValueDelegate((instance, value) => memberPropSetMethod.Invoke(instance, new object[] { value })), memberPropGetMethod.ReturnType);
        }
    }
}
