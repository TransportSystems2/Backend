using System;

namespace Common.Models
{
    public struct TimeBelt
    {
        /// <summary>
        /// The offset for daylight-savings time.
        /// </summary>
        public TimeSpan OffSet { get; set; }

        /// <summary>
        /// The offest from UTC.
        /// </summary>
        public TimeSpan RawOffset { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}