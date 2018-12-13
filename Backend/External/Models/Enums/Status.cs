namespace TransportSystems.Backend.External.Models.Enums
{
    public enum Status
    {
        /// <summary>
        /// Set when deserialization fails (default).
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Indicates that no errors occurred; the place was successfully detected and at
        /// least one result was returned.
        /// </summary>
        Ok = 1,

        /// <summary>
        /// Indicates that the requst was successful but returned no results. This may occur
        /// if the search was passed a latlng in a remote location.
        /// </summary>
        ZeroResults = 2,

        /// <summary>
        /// Indicates that you are over your quota.
        /// </summary>
        OverQueryLimit = 3,

        /// <summary>
        /// Indicates that your request was denied.
        /// </summary>
        RequestDenied = 4,

        /// <summary>
        /// Indicates that the query parameter (location or radius) is missing.
        /// </summary>
        InvalidRequest = 5,

        /// <summary>
        /// Indicates the allowed amount of elements has been exceeeded.
        /// </summary>
        MaxElementsExceeded = 6,

        /// <summary>
        /// Indicates that too many waypointss were provided in the request The maximum allowed
        /// waypoints is 8, plus the origin, and destination. ( Google Maps Premier customers
        /// may contain requests with up to 23 waypoints.)
        /// </summary>
        MaxWaypointsExceeded = 7,

        /// <summary>
        /// Indicates at least one of the locations specified in the requests's origin, destination,
        //  or waypoints could not be geocoded.
        /// </summary>
        NotFound = 8,

        /// <summary>
        /// Indicates the request could not be processed due to a server error. The request
        //  may succeed if you try again.
        /// </summary>
        UnknownError = 9,

        /// <summary>
        /// Indicates the request resulted in a Http error code.
        /// </summary>
        HttpError = 10,

        /// <summary>
        /// Indicates the request has none or an invalid key set.
        /// </summary>
        InvalidKey = 11
    }
}