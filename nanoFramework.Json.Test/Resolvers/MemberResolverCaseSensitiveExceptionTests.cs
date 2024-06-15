//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Resolvers
{
    [TestClass]
    public class MemberResolverCaseSensitiveExceptionTests
    {
        private sealed class TestClass
        {
            public int NoGetProperty { private get; set; } = 1;
            public int NoSetProperty => 1;
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldThrowPropertyWithoutGetWhenSet()
        {
            var options = new JsonSerializerOptions
            {
                ThrowExceptionWhenPropertyNotFound = true
            };

            var sut = new MemberResolver();
            Assert.ThrowsException(typeof(DeserializationException), () => sut.Get(nameof(TestClass.NoGetProperty), typeof(TestClass), options));
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldThrowPropertyWithoutSetWhenSet()
        {
            var options = new JsonSerializerOptions
            {
                ThrowExceptionWhenPropertyNotFound = true
            };

            var sut = new MemberResolver();
            Assert.ThrowsException(typeof(DeserializationException), () => sut.Get(nameof(TestClass.NoSetProperty), typeof(TestClass), options));
        }
    }
}