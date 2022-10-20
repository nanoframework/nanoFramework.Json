using nanoFramework.Json.Configuration;
using nanoFramework.TestFramework;
using System;
using System.Text;

namespace nanoFramework.Json.Test.Configuration
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void CaseSensitive_Should_BeTrueByDefault()
        {
            Assert.Equal(Settings.CaseSensitive, true);
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_BeFalseByDefault()
        {
            Assert.Equal(Settings.ThrowExceptionWhenPropertyNotFound, false);
        }
    }
}
