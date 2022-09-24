using System;

namespace nanoFramework.Json.Converters
{
    internal sealed class TimeSpanConverter : IConverter
    {
        public string ToJson(object value)
        {
            return "\"" + value.ToString() + "\"";
        }

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

            int days = 0;
            int ticks = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            // figure out where the separators are
            int indexOfFirstDot = value.IndexOf('.');
            int indexOfSecondDot = indexOfFirstDot > -1 ? value.IndexOf('.', indexOfFirstDot + 1) : -1;
            int indexOfFirstColon = value.IndexOf(':');
            int indexOfSecondColon = indexOfFirstColon > -1 ? value.IndexOf(':', indexOfFirstColon + 1) : -1;

            // sanity check for separators: all have to be ahead of string start
            if (timeSpanBits.Length > 1
                && indexOfFirstDot <= 0
                && indexOfSecondDot <= 0
                && indexOfFirstColon <= 0
                && indexOfSecondColon <= 0)
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
            if (hasDays
                && !int.TryParse(timeSpanBits[0], out days))
            {
                throw new InvalidCastException();
            }

            // bump the index if days component is present
            int processIndex = hasDays ? 1 : 0;

            if (hasHours && processIndex <= timeSpanBits.Length && 
                !int.TryParse(timeSpanBits[processIndex++], out hours))
            {
                throw new InvalidCastException();
            }

            if (hasMinutes && processIndex <= timeSpanBits.Length &&
                !int.TryParse(timeSpanBits[processIndex++], out minutes))
            {
                throw new InvalidCastException();
            }

            if (hasSeconds && processIndex <= timeSpanBits.Length &&
                !int.TryParse(timeSpanBits[processIndex++], out seconds))
            {
                throw new InvalidCastException();
            }

            if (hasTicks && processIndex <= timeSpanBits.Length)
            {
                if (!int.TryParse(timeSpanBits[processIndex], out ticks))
                {
                    throw new InvalidCastException();
                }

                // if ticks are under 999, that's milliseconds
                if (ticks < 1_000)
                {
                    ticks *= 10_000;
                }
            }

            // sanity check for valid ranges
            if ((hours >= 0 && hours < 24)
                && (minutes >= 0 && minutes < 60)
                && (seconds >= 0 && seconds < 60))
            {
                // we should have everything now
                return new TimeSpan(ticks).Add(new TimeSpan(days, hours, minutes, seconds, 0));
            }

            throw new InvalidCastException();
        }
    }
}