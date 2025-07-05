//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        [DataRow("19990101T000000")]
        [DataRow("20001120T221530")]
        [DataRow("20250706T005433")]
        [DataRow("16010101T000000")]
        [DataRow("30001231T235958")]
        public void FromiCalendar_LocalTime_ShouldNotBeMaxValue(string iCal)
        {
            var act = DateTimeExtensions.FromiCalendar(iCal);

            Assert.AreNotEqual(DateTime.MaxValue, act);
        }

        [TestMethod]
        [DataRow("19990101T000000Z")]
        [DataRow("20001120T221530Z")]
        [DataRow("20250706T005433Z")]
        [DataRow("16010101T000000Z")]
        [DataRow("30001231T235958Z")]
        public void FromiCalendar_UtcTime_ShouldNotBeMaxValue(string iCal)
        {
            var act = DateTimeExtensions.FromiCalendar(iCal);

            Assert.AreNotEqual(DateTime.MaxValue, act);
        }

        [TestMethod]
        [DataRow("12345678901234567890")]
        [DataRow("3333333333333333")]
        [DataRow("444444444444444")]
        [DataRow("12345678T901234")]
        [DataRow("000000000000000")]
        [DataRow("test")]
        [DataRow("test19990101T000000Z")]
        [DataRow("test0101T000000Z")]
        [DataRow("test19990101T000000")]
        [DataRow("test0101T000000")]
        [DataRow("_!123@1903$#*((")]
        public void FromiCalendar_NotATime_ShuldReturnMaxValue(string iCal)
        {
            var act = DateTimeExtensions.FromiCalendar(iCal);

            Assert.AreEqual(DateTime.MaxValue, act);
        }
    }
}
