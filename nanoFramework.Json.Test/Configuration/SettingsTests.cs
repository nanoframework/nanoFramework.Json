//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Configuration;
using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test.Configuration
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void CaseSensitive_Should_BeTrueByDefault()
        {
            Assert.AreEqual(Settings.CaseSensitive, true);
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_BeFalseByDefault()
        {
            Assert.AreEqual(Settings.ThrowExceptionWhenPropertyNotFound, false);
        }
    }
}
