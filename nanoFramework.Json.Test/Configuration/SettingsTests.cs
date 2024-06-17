//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.Json.Resolvers;
using nanoFramework.Json.Test.Mocks;
using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Configuration
{
    [TestClass]
    public class SettingsTests
    {
        private static bool _caseSensitive;
        private static IMemberResolver _resolver;
        private static bool _throwExceptionWhenPropertyNotFound;

        [Cleanup]
        public void Cleanup()
        {
            // Restore default settings
            JsonSerializerOptions.Default.PropertyNameCaseInsensitive = _caseSensitive;
            JsonSerializerOptions.Default.Resolver = _resolver;
            JsonSerializerOptions.Default.ThrowExceptionWhenPropertyNotFound = _throwExceptionWhenPropertyNotFound;
        }

        [Setup]
        public void Setup()
        {
            // Capture default settings
            _caseSensitive = JsonSerializerOptions.Default.PropertyNameCaseInsensitive;
            _resolver = JsonSerializerOptions.Default.Resolver;
            _throwExceptionWhenPropertyNotFound = JsonSerializerOptions.Default.ThrowExceptionWhenPropertyNotFound;
        }

        [TestMethod]
        public void CaseSensitive_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.CaseSensitive = !Settings.CaseSensitive;

            Assert.AreEqual(Settings.CaseSensitive, !JsonSerializerOptions.Default.PropertyNameCaseInsensitive);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void Resolver_Should_Delegate_To_JsonSettings()
        {
            var resolver = new MockMemberResolver();

#pragma warning disable CS0618
            Settings.Resolver = resolver;

            Assert.AreEqual(resolver, JsonSerializerOptions.Default.Resolver);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.ThrowExceptionWhenPropertyNotFound = !Settings.ThrowExceptionWhenPropertyNotFound;

            Assert.AreEqual(Settings.ThrowExceptionWhenPropertyNotFound, JsonSerializerOptions.Default.ThrowExceptionWhenPropertyNotFound);
#pragma warning restore CS0618
        }
    }
}
