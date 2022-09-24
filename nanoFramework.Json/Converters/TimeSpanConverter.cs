using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class TimeSpanConverter : IConverter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ToJson(object value)
        {
            return "\"" + value.ToString() + "\"";
        }

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
            var timeSpanBits = value.Split(':', '.');

            // sanity check
            if (timeSpanBits.Length == 0)
            {
                return TimeSpan.Zero;
            }

            // figure out where the separators are
            int indexOfFirstDot = value.IndexOf('.');
            int indexOfSecondDot = indexOfFirstDot > -1 ? value.IndexOf('.', indexOfFirstDot + 1) : -1;
            int indexOfFirstColon = value.IndexOf(':');
            int indexOfSecondColon = indexOfFirstColon > -1 ? value.IndexOf(':', indexOfFirstColon + 1) : -1;

            // sanity check for separators: all have to be ahead of string start
            if (SeparatorCheck(timeSpanBits, indexOfFirstDot, indexOfSecondDot, indexOfFirstColon, indexOfSecondColon))
            {
                throw new InvalidCastException();
            }

            // to have days, it has to have something before the 1st dot, or just have a single component
            bool hasDays = (indexOfFirstDot > 0 && indexOfFirstDot < indexOfFirstColon) || timeSpanBits.Length == 1;
            bool hasTicks = hasDays ? indexOfSecondDot > indexOfFirstDot : indexOfFirstDot > -1;
            bool hasHours = indexOfFirstColon > 0;
            bool hasMinutes = hasHours && indexOfFirstColon > -1;
            bool hasSeconds = hasMinutes && indexOfSecondColon > -1;

            // sanity check for ticks without other time components
            if (hasTicks && !hasHours)
            {
                throw new InvalidCastException();
            }

            // let the parsing start!
            int days = 0;
            if (hasDays
                && !int.TryParse(timeSpanBits[0], out days))
            {
                throw new InvalidCastException();
            }

            // bump the index if days component is present
            int processIndex = hasDays ? 1 : 0;

            var hours = ParseValueFromString(hasHours, timeSpanBits, ref processIndex);
            var minutes = ParseValueFromString(hasMinutes, timeSpanBits, ref processIndex);
            var seconds = ParseValueFromString(hasSeconds, timeSpanBits, ref processIndex);
            var ticks = HandleTicks(timeSpanBits, hasTicks, processIndex, ticks);

            // sanity check for valid ranges
            if (IsInvalidTimeSpan(hours, minutes, seconds))
            {
                throw new InvalidCastException();
            }

            // we should have everything now
            return new TimeSpan(ticks).Add(new TimeSpan(days, hours, minutes, seconds, 0));
        }

        private static int HandleTicks(string[] timeSpanBits, bool hasTicks, int processIndex)
        {
            if (!hasTicks || processIndex > timeSpanBits.Length)
            {
                return 0;
            }

            if (!int.TryParse(timeSpanBits[processIndex], out var ticks))
            {
                throw new InvalidCastException();
            }

            // if ticks are under 999, that's milliseconds
            if (ticks < 1_000)
            {
                ticks *= 10_000;
            }

            return ticks;
        }

        private static bool SeparatorCheck(string[] timeSpanBits, int indexOfFirstDot, int indexOfSecondDot, int indexOfFirstColon, int indexOfSecondColon)
        {
            return timeSpanBits.Length > 1
                            && indexOfFirstDot <= 0
                            && indexOfSecondDot <= 0
                            && indexOfFirstColon <= 0
                            && indexOfSecondColon <= 0;
        }

        private static int ParseValueFromString(bool hasValue,string[] timeSpanBits, ref int processIndex)
        {
            if (!hasValue)
            {
                return 0;
            }

            if (processIndex > timeSpanBits.Length)
            {
                return 0;
            }

            if (!int.TryParse(timeSpanBits[processIndex++], out var value))
            {
                throw new InvalidCastException();
            }

            return value;
        }

        private static bool IsInvalidTimeSpan(int hour, int minutes, int seconds)
        {
            if (hour < 0 || hour >= 24)
            {
                return true;
            }

            if (minutes < 0 || minutes >= 60)
            {
                return true;
            }

            if (seconds < 0 || seconds >= 60)
            {
                return true;
            }

            return false;
        }
    }
}