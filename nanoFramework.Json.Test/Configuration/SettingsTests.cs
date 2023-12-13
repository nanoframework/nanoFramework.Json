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
            JsonSettings.CaseSensitive = _caseSensitive;
            JsonSettings.Resolver = _resolver;
            JsonSettings.ThrowExceptionWhenPropertyNotFound = _throwExceptionWhenPropertyNotFound;
        }

        [Setup]
        public void Setup()
        {
            // Capture default settings
            _caseSensitive = JsonSettings.CaseSensitive;
            _resolver = JsonSettings.Resolver;
            _throwExceptionWhenPropertyNotFound = JsonSettings.ThrowExceptionWhenPropertyNotFound;
        }
        
        [TestMethod]
        public void CaseSensitive_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.CaseSensitive = !JsonSettings.CaseSensitive;

            Assert.AreEqual(Settings.CaseSensitive, JsonSettings.CaseSensitive);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void Resolver_Should_Delegate_To_JsonSettings()
        {
            var resolver = new MockMemberResolver();

#pragma warning disable CS0618
            Settings.Resolver = resolver;

            Assert.AreEqual(resolver, JsonSettings.Resolver);
#pragma warning restore CS0618
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_Delegate_To_JsonSettings()
        {
#pragma warning disable CS0618
            Settings.ThrowExceptionWhenPropertyNotFound = !JsonSettings.ThrowExceptionWhenPropertyNotFound;

            Assert.AreEqual(Settings.ThrowExceptionWhenPropertyNotFound, JsonSettings.ThrowExceptionWhenPropertyNotFound);
#pragma warning restore CS0618
        }
    }
}
