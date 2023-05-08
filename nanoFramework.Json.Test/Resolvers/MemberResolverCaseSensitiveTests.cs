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
    public class MemberResolverCaseSensitiveTests
    {
        private sealed class TestClass
        {
            public int TestField = 1;
            public int TestProperty { get; set; } = 1;
            public int NoGetProperty { private get; set; } = 1;
            public int NoSetProperty { get; } = 1;
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldResolveField()
        {
            var resolver = new MemberResolver();
            var classInstance = new TestClass();
            var valueToSet = 5;

            var member = resolver.Get(nameof(TestClass.TestField), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.AreEqual(classInstance.TestField, valueToSet);
            Assert.AreEqual(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldResolveProperty()
        {
            var resolver = new MemberResolver();
            var classInstance = new TestClass();
            var valueToSet = 6;

            var member = resolver.Get(nameof(TestClass.TestProperty), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.AreEqual(classInstance.TestProperty, valueToSet);
            Assert.AreEqual(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldSkipPropertyWithoutGet()
        {
            var resolver = new MemberResolver();

            var member = resolver.Get(nameof(TestClass.NoGetProperty), typeof(TestClass));

            Assert.True(member.Skip);
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldSkipPropertyWithoutSet()
        {
            var resolver = new MemberResolver();

            var member = resolver.Get(nameof(TestClass.NoSetProperty), typeof(TestClass));

            Assert.True(member.Skip);
        }
    }
}
