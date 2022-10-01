using System;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    public delegate void SetValueDelegate(object objectInstance, object valueToSet);

    public struct MemberSet
    {
        public SetValueDelegate SetValue { get; }
        public Type ObjectType { get; }

        public MemberSet(SetValueDelegate setValue, Type objectType)
        {
            SetValue = setValue;
            ObjectType = objectType;
        }
    }

    public interface IMemberResolver
    {
        MemberSet GetResolver(string memberName, Type objectType);
    }
}
