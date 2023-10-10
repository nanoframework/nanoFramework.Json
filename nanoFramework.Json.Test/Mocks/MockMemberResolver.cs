using System;
using nanoFramework.Json.Resolvers;

namespace nanoFramework.Json.Test.Mocks
{
    internal class MockMemberResolver: IMemberResolver
    {
        public MemberSet Get(string memberName, Type objectType)
        {
            return new MemberSet(true);
        }
    }
}
