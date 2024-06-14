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
            JsonSerializerOptions.PropertyNameCaseInsensitive = _caseSensitive;
            JsonSerializerOptions.Resolver = _resolver;
            JsonSerializerOptions.ThrowExceptionWhenPropertyNotFound = _throwExceptionWhenPropertyNotFound;
        }

        [Setup]
        public void Setup()
        {
            // Capture default settings
            _caseSensitive = JsonSerializerOptions.PropertyNameCaseInsensitive;
            _resolver = JsonSerializerOptions.Resolver;
            _throwExceptionWhenPropertyNotFound = JsonSerializerOptions.ThrowExceptionWhenPropertyNotFound;
        }
        
        [TestMethod]
        public void CaseSensitive_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.CaseSensitive = !JsonSerializerOptions.PropertyNameCaseInsensitive;

            Assert.AreEqual(Settings.CaseSensitive, !JsonSerializerOptions.PropertyNameCaseInsensitive);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void Resolver_Should_Delegate_To_JsonSettings()
        {
            var resolver = new MockMemberResolver();

#pragma warning disable CS0618
            Settings.Resolver = resolver;

            Assert.AreEqual(resolver, JsonSerializerOptions.Resolver);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.ThrowExceptionWhenPropertyNotFound = !JsonSerializerOptions.ThrowExceptionWhenPropertyNotFound;

            Assert.AreEqual(Settings.ThrowExceptionWhenPropertyNotFound, JsonSerializerOptions.ThrowExceptionWhenPropertyNotFound);
#pragma warning restore CS0618
        }
    }
}
