//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
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
            public int TestProperty { get; set; } = 1;
        }

        [Setup]
        public void MemberResolverCaseInsensitiveTests_Setup()
        {
            Settings.CaseSensitive = false;
        }

        [Cleanup]
        public void MemberResolverCaseInsensitive_Cleanup()
        {
            Settings.CaseSensitive = true;
        }

        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldReturnCaseInsensitivePropertyWhenSet()
        {
            var resolver = new MemberResolver();
            var classInstance = new TestClass();
            var valueToSet = 6;

            var member = resolver.Get(nameof(TestClass.TestProperty).ToLower(), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestProperty, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }


        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldReturnCaseInsensitiveFieldWhenSet()
        {
            var resolver = new MemberResolver();
            var classInstance = new TestClass();
            var valueToSet = 5;

            var member = resolver.Get(nameof(TestClass.TestField).ToLower(), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestField, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void MemberResolverCaseInsensitive_Get_ShouldSkipWhenNotFoundCaseInsensitiveProperty()
        {
            var resolver = new MemberResolver();

            var member = resolver.Get("NotExistingProperty", typeof(TestClass));

            Assert.Equal(member.Skip, true);
        }
    }
}
