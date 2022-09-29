using System;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    internal class PropertyResolver : IMemberResolver
    {
        private MethodInfo memberPropGetMethod;
        private MethodInfo memberPropSetMethod;

        public PropertyResolver(MethodInfo memberPropGetMethod, MethodInfo memberPropSetMethod)
        {
            this.memberPropGetMethod = memberPropGetMethod;
            this.memberPropSetMethod = memberPropSetMethod;
        }

        public Type GetMemberType()
        {
            return memberPropGetMethod.ReturnType;
        }

        public void SetValue(object rootInstance, object memberObject)
        {
            memberPropSetMethod.Invoke(rootInstance, new object[] { memberObject });
        }
    }
}
