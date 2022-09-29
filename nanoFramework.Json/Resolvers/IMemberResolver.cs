using System;
using System.Text;

namespace nanoFramework.Json.Resolvers
{
    internal interface IMemberResolver
    {
        Type GetMemberType();
        void SetValue(object rootInstance, object memberObject);
    }
}
