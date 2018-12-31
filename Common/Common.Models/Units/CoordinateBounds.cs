namespace Common.Models.Units
{
    public class CoordinateBounds
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Distance { get; set; }

        /// <summary>
        /// The upper boundary latitude point in decimal notation
        /// </summary>
        public double MinLatitude { get; set; }

        /// <summary>
        /// The left boundary longitude point in decimal notation
        /// </summary>
        public double MinLongitude { get; set; }

        /// <summary>
        /// The lower boundary latitude point in decimal notation
        /// </summary>
        public double MaxLatitude { get; set; }

        /// <summary>
        /// The right boundary longitude point in decimal notation
        /// </summary>
        public double MaxLongitude { get; set; }
    }
}