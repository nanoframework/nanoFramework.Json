using System;
using System.Reflection;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    internal sealed class FieldResolver : IMemberResolver
    {
        private readonly FieldInfo memberFieldInfo;

        public FieldResolver(FieldInfo memberFieldInfo)
        {
            this.memberFieldInfo = memberFieldInfo;
        }

        public Type GetMemberType()
        {
            return memberFieldInfo.FieldType;
        }

        public void SetValue(object rootInstance, object memberObject)
        {
            memberFieldInfo.SetValue(rootInstance, memberObject);
        }
    }
}
