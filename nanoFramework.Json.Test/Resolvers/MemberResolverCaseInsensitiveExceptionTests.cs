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
    public class MemberResolverCaseInsensitiveExceptionTests
    {
        private sealed class TestClass
        {
            public int NoGetProperty { private get; set; } = 1;
            public int NoSetProperty { get; } = 1;
        }

        [Setup]
        public void MemberResolverCaseInsensitiveExceptionTests_Setup()
        {
            Settings.ThrowExceptionWhenPropertyNotFound = true;
            Settings.CaseSensitive = false;
        }

        [Cleanup]
        public void MemberResolverCaseInsensitiveExceptionTests_Cleanup()
        {
            Settings.ThrowExceptionWhenPropertyNotFound = false;
            Settings.CaseSensitive = true;
        }

        [TestMethod]
        public void MemberResolverCaseInsensitiveExceptionTests_Get_ShouldSkipPropertyWithoutGet()
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
        public void MemberResolverCaseInsensitiveExceptionTests_Get_ShouldSkipPropertyWithoutSet()
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
