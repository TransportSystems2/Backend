namespace TransportSystems.Backend.Core.Domain.Core.Trading
{
    public enum LotStatus
    {
        /// <summary>
        /// New
        /// </summary>
        New,

        /// <summary>
        /// Активный (торгуется)
        /// </summary>
        Traded,

        /// <summary>
        /// Истекло время
        /// </summary>
        Expired,

        /// <summary>
        /// Удачно завершен
        /// </summary>
        Won,

        /// <summary>
        /// Отменен
        /// </summary>
        Canceled
    }
}