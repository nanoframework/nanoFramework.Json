//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class TimeSpanConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value) => $"\"{value}\"";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object ToType(object value)
        {
            return ConvertFromString(value.ToString());
        }

        /// <summary>
        /// Try converting a string value to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value"><see cref="string"/> to convert.</param>
        /// <returns><see langword="true"/> if conversion was successful. <see langword="false"/> otherwise.</returns>
        internal static TimeSpan ConvertFromString(string value)
        {
            // split string value with all possible separators
            // format is: -ddddd.HH:mm:ss.fffffff
            var tokens = value.Split(':', '.');

            // sanity check
            if (tokens.Length == 0)
            {
                return TimeSpan.Zero;
            }

            // figure out where the separators are
            var indexOfFirstDot = value.IndexOf('.');
            var indexOfSecondDot = indexOfFirstDot > -1 ? value.IndexOf('.', indexOfFirstDot + 1) : -1;
            var indexOfFirstColon = value.IndexOf(':');
            var indexOfSecondColon = indexOfFirstColon > -1 ? value.IndexOf(':', indexOfFirstColon + 1) : -1;

            // sanity check for separators: all have to be ahead of string start
            if (SeparatorCheck(tokens, indexOfFirstDot, indexOfSecondDot, indexOfFirstColon, indexOfSecondColon))
            {
                throw new InvalidCastException();
            }

            var isNegative = value.StartsWith("-");

            // to have days, it has to have something before the 1st dot, or just have a single component
            var hasDays = (indexOfFirstDot > 0 && indexOfFirstDot < indexOfFirstColon) || tokens.Length == 1;
            var hasTicks = hasDays ? indexOfSecondDot > indexOfFirstDot : indexOfFirstDot > -1;
            var hasHours = indexOfFirstColon > 0;
            var hasMinutes = hasHours && indexOfFirstColon > -1;
            var hasSeconds = hasMinutes && indexOfSecondColon > -1;

            // sanity check for ticks without other time components
            if (hasTicks && !hasHours)
            {
                throw new InvalidCastException();
            }

            // let the parsing start!
            var tokenIndex = 0;

            var days = ParseToken(hasDays, tokens, ref tokenIndex);
            var hours = ParseToken(hasHours, tokens, ref tokenIndex);
            var minutes = ParseToken(hasMinutes, tokens, ref tokenIndex);
            var seconds = ParseToken(hasSeconds, tokens, ref tokenIndex);
            var ticks = ParseTicks(hasTicks, tokens, tokenIndex);

            // sanity check for valid ranges
            if (IsInvalidTimeSpan(hours, minutes, seconds))
            {
                throw new InvalidCastException();
            }

            // we should have everything now
            var timeSpan = new TimeSpan(days, hours, minutes, seconds, 0).Add(new TimeSpan(ticks));

            return isNegative ? -timeSpan : timeSpan;
        }

        private static bool IsInvalidTimeSpan(int hour, int minutes, int seconds)
        {
            if (hour is < 0 or > 24)
            {
                return true;
            }

            if (minutes is < 0 or >= 60)
            {
                return true;
            }

            if (seconds is < 0 or >= 60)
            {
                return true;
            }

            return false;
        }

        private static int ParseTicks(bool hasTicks, string[] tokens, int tokenIndex)
        {
            if (!hasTicks || tokenIndex > tokens.Length)
            {
                return 0;
            }

            var token = tokens[tokenIndex];

            if (token.Length > 7)
            {
                token = token.Substring(0, 7);
            }

            if (!int.TryParse(token, out var value))
            {
                throw new InvalidCastException();
            }

            value = token.Length switch
            {
                1 => value * 1_000_000,
                2 => value * 100_000,
                3 => value * 10_000,
                4 => value * 1_000,
                5 => value * 100,
                6 => value * 10,
                _ => value
            };

            return value >= 0 ? value : value * -1;
        }

        private static int ParseToken(bool hasValue, string[] tokens, ref int tokenIndex)
        {
            if (!hasValue || tokenIndex > tokens.Length)
            {
                return 0;
            }

            if (!int.TryParse(tokens[tokenIndex++], out var value))
            {
                throw new InvalidCastException();
            }

            return value >= 0 ? value : value * -1;
        }

        private static bool SeparatorCheck(string[] tokens, int indexOfFirstDot, int indexOfSecondDot, int indexOfFirstColon, int indexOfSecondColon)
        {
            return tokens.Length > 1
                   && indexOfFirstDot <= 0
                   && indexOfSecondDot <= 0
                   && indexOfFirstColon <= 0
                   && indexOfSecondColon <= 0;
        }
    }
}
