namespace GPlusQuickstartCsharp
{
    using System;

    /// <summary>
    /// Helps converting between miliseconds since 1970-1-1 and C# <c>DateTime</c> object.
    /// </summary>
    public class googleTime
    {
        private static readonly DateTime zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets the total milliseconds.
        /// </summary>
        /// <value>
        /// The total milliseconds.
        /// </value>
        public long TotalMilliseconds { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="googleTime"/> class from being created.
        /// </summary>
        private googleTime()
        {
        }

        /// <summary>
        /// Create a time object from the given date time.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <returns>The time object.</returns>
        public static googleTime FromDateTime(DateTime dt)
        {
            if (dt < zero)
            {
                throw new Exception("Invalid Google datetime.");
            }

            return new googleTime
            {
                TotalMilliseconds = (long)(dt - zero).TotalMilliseconds,
            };
        }

        /// <summary>
        /// Creates a time object from the given nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The ns.</param>
        /// <returns>The time object.</returns>
        public static googleTime FromNanoseconds(long? nanoseconds)
        {
            if (nanoseconds < 0)
            {
                throw new ArgumentOutOfRangeException("Must be greater than 0.");
            }

            return new googleTime
            {
                TotalMilliseconds = (long)(nanoseconds.GetValueOrDefault(0) / 1000000)
            };
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public static googleTime Now
        {
            get { return FromDateTime(DateTime.Now); }
        }

        /// <summary>
        /// Adds the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        public googleTime Add(TimeSpan timeSpan)
        {
            return new googleTime
            {
                TotalMilliseconds = this.TotalMilliseconds + (long)timeSpan.TotalMilliseconds
            };
        }

        /// <summary>
        /// Converts this instance into a <c>DateTime</c> object.
        /// </summary>
        /// <returns>The date time.</returns>
        public DateTime ToDateTime()
        {
            return zero.AddMilliseconds(this.TotalMilliseconds);
        }
    }
}