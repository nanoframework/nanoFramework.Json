//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Resolvers
{
    [TestClass]
    public class CaseSensitiveResolverTests
    {
        private class TestClass
        {
            public int TestField;
            public int TestProperty { get; set; }
            public int SkipProperty { private get;  set; }
            public int ThrowProperty { get; }
        }

        [TestMethod]
        public void CaseSensitiveResolver_Get_ShouldResolveField()
        {
            var resolver = new CaseSensitiveResolver();
            var classInstance = new TestClass();
            var valueToSet = 5;

            var member = resolver.Get(nameof(TestClass.TestField), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestField, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void CaseSensitiveResolver_Get_ShouldResolveProperty()
        {
            var resolver = new CaseSensitiveResolver();
            var classInstance = new TestClass();
            var valueToSet = 6;

            var member = resolver.Get(nameof(TestClass.TestProperty), typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestProperty, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void CaseSensitiveResolver_Get_ShouldSkipPropertyWithoutGet()
        {
            var resolver = new CaseSensitiveResolver();

            var member = resolver.Get(nameof(TestClass.SkipProperty), typeof(TestClass));

            Assert.True(member.Skip);
        }

        [TestMethod]
        public void CaseSensitiveResolver_Get_ShouldThrowForPropertyWithNoSet()
        {
            var resolver = new CaseSensitiveResolver();

            try
            {
                var member = resolver.Get(nameof(TestClass.ThrowProperty), typeof(TestClass));
            }
            catch (DeserializationException)
            {
                // Intended. Method should throw this type of exception when no set method.
                return;
            }

            throw new Exception($"Should throw {nameof(DeserializationException)}.");
        }
    }
}
