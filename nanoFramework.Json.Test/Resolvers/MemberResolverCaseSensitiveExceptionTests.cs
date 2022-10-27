//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Resolvers
{
    [TestClass]
    public class MemberResolverCaseSensitiveExceptionTests
    {
        private sealed class TestClass
        {
            public int NoGetProperty { private get; set; } = 1;
            public int NoSetProperty { get; } = 1;
        }

        [Setup]
        public void MemberResolverCaseSensitiveExceptionTests_Setup()
        {
            Settings.ThrowExceptionWhenPropertyNotFound = true;
        }

        [Cleanup]
        public void MemberResolverCaseSensitiveExceptionTests_Cleanup()
        {
            Settings.ThrowExceptionWhenPropertyNotFound = false;
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldThrowPropertyWithoutGetWhenSet()
        {
            var resolver = new MemberResolver();

            try
            {
                resolver.Get(nameof(TestClass.NoGetProperty), typeof(TestClass));
            }
            catch (DeserializationException)
            {
                // Intended. Method should throw this type of exception when no set method.
                return;
            }

            throw new InvalidOperationException($"Should throw {nameof(DeserializationException)}.");
        }

        [TestMethod]
        public void MemberResolverCaseSensitive_Get_ShouldThrowPropertyWithoutSetWhenSet()
        {
            var resolver = new MemberResolver();

            try
            {
                resolver.Get(nameof(TestClass.NoSetProperty), typeof(TestClass));
            }
            catch (DeserializationException)
            {
                // Intended. Method should throw this type of exception when no set method.
                return;
            }

            throw new InvalidOperationException($"Should throw {nameof(DeserializationException)}.");
        }
    }
}
