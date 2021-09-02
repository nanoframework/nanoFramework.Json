//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json
{
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a vCal or iCal date string into a DateTime object.
        /// </summary>
        /// <param name="iCalendar"></param>
        /// <returns></returns>
        public static DateTime FromiCalendar(string iCalendar)
        {
            string result;

            if ((iCalendar.Contains("DTSTART")) || (iCalendar.Contains("DTEND")))
            {
                result = iCalendar.Split(':')[1];
            }
            else
            {
                result = iCalendar;
            }

            // Check to see if format contains the timezone ID, or contains UTC reference
            // Neither means it's local time
            bool tzid = iCalendar.Contains("TZID");
            bool utc = iCalendar.EndsWith("Z");
            string time;

            if (tzid)
            {
                string[] parts = iCalendar.Split(new char[] { ';', '=', ':' });

                // parts[0] == DTSTART or DTEND
                // parts[1] == TZID
                // parts[2] == the timezone string
                // parts[3] == localtime string
                _ = parts[2];
                time = parts[3];
            }
            else if (utc)
            {
                time = result.Substring(0, result.Length - 1);  // truncate the trailing 'Z'
            }
            else
            {
                time = result;  // localtime
            }

            // We now have the time string to parse, and we'll adjust
            // to UTC or timezone after parsing
            string year = time.Substring(0, 4);
            string month = time.Substring(4, 2);
            string day = time.Substring(6, 2);
            string hour = time.Substring(9, 2);
            string minute = time.Substring(11, 2);
            string second = time.Substring(13, 2);

            // Check if any of the date time parts is non-numeric
            if (!IsNumeric(year))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(month))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(day))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(hour))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(minute))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(second))
            {
                return DateTime.MaxValue;
            }

            DateTime dt = new(
                Convert.ToInt32(year),
                Convert.ToInt32(month),
                Convert.ToInt32(day),
                Convert.ToInt32(hour),
                Convert.ToInt32(minute),
                Convert.ToInt32(second));

            if (utc)
            {
                // Convert the Kind to DateTimeKind.Utc
                dt = new DateTime(0, DateTimeKind.Utc).AddTicks(dt.Ticks);
            }
            else if (tzid)
            {
                // not sure what to do here
            }

            return dt;
        }

        /// <summary>
        /// Converts an ISO 8601 time/date format string, which is used by JSON and others,
        /// into a DateTime object.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromIso8601(string date)
        {

            // Check to see if format contains the timezone ID, or contains UTC reference
            // Neither means it's local time
            bool utc = date.EndsWith("Z");

            string[] parts = date.Split(new char[] { 'T', 'Z', ':', '-', '.', '+', });

            // We now have the time string to parse, and we'll adjust
            // to UTC or timezone after parsing
            string year = parts[0];
            string month = (parts.Length > 1) ? parts[1] : "1";
            string day = (parts.Length > 2) ? parts[2] : "1";
            string hour = (parts.Length > 3) ? parts[3] : "0";
            string minute = (parts.Length > 4) ? parts[4] : "0";
            string second = (parts.Length > 5) ? parts[5] : "0";
            string ms = (parts.Length > 6) ? parts[6] : "0";

            // Check if any of the date time parts is non-numeric
            if (!IsNumeric(year))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(month))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(day))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(hour))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(minute))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(second))
            {
                return DateTime.MaxValue;
            }
            else if (!IsNumeric(ms))
            {
                return DateTime.MaxValue;
            }

            // sanity check for bad milliseconds format
            int milliseconds = Convert.ToInt32(ms);

            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            DateTime dt = new(
                Convert.ToInt32(year),
                Convert.ToInt32(month),
                Convert.ToInt32(day),
                Convert.ToInt32(hour),
                Convert.ToInt32(minute),
                Convert.ToInt32(second),
                milliseconds);

            if (utc)
            {
                // Convert the Kind to DateTimeKind.Utc if string Z present
                dt = new DateTime(dt.Ticks, DateTimeKind.Utc); //TODO!!!
            }
            else
            {
                //nF does not support non UTC dates, so should we throw an exception instead.
                throw new NotSupportedException();
            }

            return dt;
        }

        /// <summary>
        /// Converts a DateTime object into an ISO 8601 string.  This version
        /// always returns the string in UTC format.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToIso8601(DateTime dt)
        {
            string result = dt.Year.ToString() + "-" +
                            TwoDigits(dt.Month) + "-" +
                            TwoDigits(dt.Day) + "T" +
                            TwoDigits(dt.Hour) + ":" +
                            TwoDigits(dt.Minute) + ":" +
                            TwoDigits(dt.Second) + "." +
                            ThreeDigits(dt.Millisecond) + "Z";

            return result;
        }

        /// <summary>
        /// Returns true if the given string contains only numeric characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsNumeric(string str)
        {
            foreach (char c in str)
            {
                if (!((c >= '0') && (c <= '9')))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Ensures a two-digit number with leading zero if necessary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string TwoDigits(int value)
        {
            if (value < 10)
            {
                return "0" + value.ToString();
            }

            return value.ToString();

        }

        /// <summary>
        /// Ensures a three-digit number with leading zeros if necessary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ThreeDigits(int value)
        {
            if (value < 10)
            {
                return "00" + value.ToString();
            }
            else if (value < 100)
            {
                return "0" + value.ToString();
            }

            return value.ToString();
        }

        /// <summary>
        /// The ASP.NET Ajax team made up their own time date format for JSON strings, and it's
        /// explained in this article: http://msdn.microsoft.com/en-us/library/bb299886.aspx
        /// Converts a DateTime to the ASP.NET Ajax JSON format.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToASPNetAjax(DateTime dt)
        {
            string value = dt.Ticks.ToString();

            return @"\/Date(" + value + @")\/";
        }

        /// <summary>
        /// Converts an ASP.NET Ajaz JSON string to DateTime
        /// </summary>
        /// <param name="ajax"></param>
        /// <returns></returns>
        public static DateTime FromASPNetAjax(string ajax)
        {
            string[] parts = ajax.Split(new char[] { '(', ')' });

            // Check if any of the date time parts is non-numeric
            if (parts.Length < 2)
            {
                return DateTime.MaxValue;
            }

            if (!IsNumeric(parts[1]))
            {
                return DateTime.MaxValue;
            }

            long ticks = Convert.ToInt64(parts[1]);

            // Create a Utc DateTime based on the tick count
            DateTime dt = new(ticks, DateTimeKind.Utc);

            return dt;
        }

        internal static DateTime ConvertFromString(string value)
        {
            // check if this could be a DateTime value
            // min lenght is 18 for Java format: "Date(628318530718)": 18

            DateTime dtValue = DateTime.MaxValue;

            if (value.Length >= 18)
            {
                // check for special case of "null" date
                if (value == "0001-01-01T00:00:00Z")
                {
                    dtValue = DateTime.MinValue;
                }

                if (dtValue == DateTime.MaxValue)
                {
                    try
                    {
                        dtValue = FromIso8601(value);
                    }
                    catch
                    {
                        // intended, to catch failed conversion attempt
                    }
                }

                if (dtValue == DateTime.MaxValue)
                {
                    try
                    {
                        dtValue = FromASPNetAjax(value);
                    }
                    catch
                    {
                        // intended, to catch failed conversion attempt
                    }
                }

                if (dtValue == DateTime.MaxValue)
                {
                    try
                    {
                        dtValue = FromiCalendar(value);
                    }
                    catch
                    {
                        // intended, to catch failed conversion attempt
                    }
                }
            }

            return dtValue;
        }
    }
}
