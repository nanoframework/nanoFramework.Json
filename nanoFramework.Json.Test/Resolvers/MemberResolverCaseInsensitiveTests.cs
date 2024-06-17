//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Resolvers
{
    [TestClass]
    public class MemberResolverCaseInsensitiveTests
    {
        private sealed class TestClass
        {
            public int TestField = 1;
            public int TestProperty { get; init; } = 1;
        }

        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldReturnCaseInsensitivePropertyWhenSet()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var sut = new MemberResolver();
            var classInstance = new TestClass();
            const int valueToSet = 6;

            var member = sut.Get(nameof(TestClass.TestProperty).ToLower(), typeof(TestClass), options);
            member.SetValue(classInstance, valueToSet);

            Assert.AreEqual(classInstance.TestProperty, valueToSet);
            Assert.AreEqual(member.ObjectType.FullName, typeof(int).FullName);
        }


        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldReturnCaseInsensitiveFieldWhenSet()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var sut = new MemberResolver();
            var classInstance = new TestClass();
            const int valueToSet = 5;

            var member = sut.Get(nameof(TestClass.TestField).ToLower(), typeof(TestClass), options);
            member.SetValue(classInstance, valueToSet);

            Assert.AreEqual(classInstance.TestField, valueToSet);
            Assert.AreEqual(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldSkipWhenNotFoundCaseInsensitiveProperty()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var sut = new MemberResolver();

            var member = sut.Get("NotExistingProperty", typeof(TestClass), options);

            Assert.IsTrue(member.Skip);
        }
    }
}
