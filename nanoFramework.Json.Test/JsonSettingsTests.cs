//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonSettingsTests
    {
        [TestMethod]
        public void PropertyNameCaseInsensitive_Should_Be_False_By_Default()
        {
            Assert.IsFalse(JsonSerializerOptions.Default.PropertyNameCaseInsensitive);
        }

        [TestMethod]
        public void ThrowExceptionWhenPropertyNotFound_Should_Be_False_By_Default()
        {
            Assert.IsFalse(JsonSerializerOptions.Default.PropertyNameCaseInsensitive);
        }
    }
}
