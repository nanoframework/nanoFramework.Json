//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Converters;
using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Converters
{
    [TestClass]
    public class TimeSpanConverterTests
    {
        [TestMethod]
        public void TimeSpanConverter_ToType_Should_Return_Valid_Data()
        {
            var values = new[]
            {
                "-1.02:03:04.005",
                "1.02:03:04.0050000",
                "4.03:02:01.654321",
                "4.03:02:01.65432",
                "4.03:02:01.6543",
                "4.03:02:01.654",
                "4.03:02:01.65",
                "4.03:02:01.6",
                "04:20:19",
                "07:32",
            };

            var expected = new[]
            {
                -new TimeSpan(1, 2, 3, 4, 5),
                new TimeSpan(1, 2, 3, 4, 5),
                new TimeSpan(4, 3, 2, 1, 654).Add(new TimeSpan(3210)),
                new TimeSpan(4, 3, 2, 1, 654).Add(new TimeSpan(3200)),
                new TimeSpan(4, 3, 2, 1, 654).Add(new TimeSpan(3000)),
                new TimeSpan(4, 3, 2, 1, 654),
                new TimeSpan(4, 3, 2, 1, 650),
                new TimeSpan(4, 3, 2, 1, 600),
                new TimeSpan(4, 20, 19),
                new TimeSpan(7, 32, 0),
            };

            var sut = new TimeSpanConverter();

            for (var i = 0; i < values.Length; i++)
            {
                var actual = (TimeSpan) sut.ToType(values[i]);

                Assert.AreEqual(expected[i], actual);
            }
        }

        [TestMethod]
        public void TimeSpanConverter_ToJson_Should_Return_Valid_Data()
        {
            var values = new[]
            {
                -new TimeSpan(1, 2, 3, 4, 5),
                new TimeSpan(1, 2, 3, 4, 5),
                new TimeSpan(4, 3, 2, 1, 654).Add(new TimeSpan(3210)),
                new TimeSpan(4, 20, 19),
                new TimeSpan(7, 32, 0),
                new TimeSpan(0, 29, 0),
            };

            var expected = new[]
            {
                "\"-1.02:03:04.0050000\"",
                "\"1.02:03:04.0050000\"",
                "\"4.03:02:01.6543210\"",
                "\"04:20:19\"",
                "\"07:32:00\"",
                "\"00:29:00\"",
            };

            var sut = new TimeSpanConverter();

            for (var i = 0; i < values.Length; i++)
            {
                var actual = sut.ToJson(values[i]);

                Assert.AreEqual(expected[i], actual);
            }
        }
    }
}
