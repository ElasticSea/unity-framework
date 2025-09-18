using System;
using System.Text;

namespace ElasticSea.Framework.Extensions
{
    public static class TimeSpanExtensions
    {
        // Julian year ≈ 365.2425 days (standard in astronomy)
        private const double DaysPerJulianYear = 365.2425;

        private static readonly long TicksPerMicrosecond = 10; // 1 µs = 10 ticks
        private static readonly long TicksPerMillisecond = TimeSpan.TicksPerMillisecond; // 10_000
        private static readonly long TicksPerSecond = TimeSpan.TicksPerSecond; // 10_000_000
        private static readonly long TicksPerMinute = TimeSpan.TicksPerMinute;
        private static readonly long TicksPerHour = TimeSpan.TicksPerHour;
        private static readonly long TicksPerDay = TimeSpan.TicksPerDay;
        private static readonly long TicksPerYear = (long)Math.Round(DaysPerJulianYear * TimeSpan.TicksPerDay); // ~31_556_952_000_000

        /// <summary>
        /// Formats a TimeSpan using scientific time units from ns up to years (Julian), skipping weeks and months.
        /// Shows only non-zero parts, from largest to smallest unit present. Example: "1 year 2 days 3 h 4 min 5 s 6 ms 7 µs 800 ns".
        /// </summary>
        /// <param name="ts">TimeSpan to format</param>
        /// <param name="maxParts">Limit number of displayed parts (e.g., 3 => "1 year 2 days 3 h"). Use int.MaxValue for all.</param>
        /// <param name="shortUnits">Use short unit symbols (h, min, s, ms, µs, ns) instead of words.</param>
        /// <returns>Human-friendly scientific string.</returns>
        public static string ToScientificString(this TimeSpan ts, int maxParts = int.MaxValue, bool shortUnits = true)
        {
            if (ts.Ticks == 0) return "0 ns"; // choose a consistent zero

            bool negative = ts.Ticks < 0;
            long ticks = Math.Abs(ts.Ticks);

            // Decompose using integer tick math (avoids overflow):
            long years = ticks / TicksPerYear;
            ticks %= TicksPerYear;
            long days = ticks / TicksPerDay;
            ticks %= TicksPerDay;
            long hours = ticks / TicksPerHour;
            ticks %= TicksPerHour;
            long minutes = ticks / TicksPerMinute;
            ticks %= TicksPerMinute;
            long seconds = ticks / TicksPerSecond;
            ticks %= TicksPerSecond;
            long milliseconds = ticks / TicksPerMillisecond;
            ticks %= TicksPerMillisecond;
            long microseconds = ticks / TicksPerMicrosecond;
            ticks %= TicksPerMicrosecond;
            long nanoseconds = ticks * 100; // 1 tick = 100 ns; remainder < 10 ticks, so safe

            var sb = new StringBuilder(64);
            if (negative) sb.Append('-');

            int written = 0;

            void Append(long val, string longName, string shortName)
            {
                if (written >= maxParts || val == 0) return;
                if (written > 0) sb.Append(' ');
                if (shortUnits)
                {
                    sb.Append(val).Append(' ').Append(shortName);
                }
                else
                {
                    sb.Append(val).Append(' ').Append(val == 1 ? longName : longName + "s");
                }

                written++;
            }

            // Largest → smallest
            Append(years, "year", "yr");
            Append(days, "day", "d");
            Append(hours, "hour", "h");
            Append(minutes, "minute", "min");
            Append(seconds, "second", "s");
            Append(milliseconds, "millisecond", "ms");
            Append(microseconds, "microsecond", "µs");
            Append(nanoseconds, "nanosecond", "ns");

            // If we limited parts and ended early, we’re done.
            // If nothing was appended (extremely tiny but nonzero), show ns.
            if (written == 0) sb.Append("0 ns");
            return sb.ToString();
        }
    }
}