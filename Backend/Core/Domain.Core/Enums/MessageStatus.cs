namespace TransportSystems.Backend.Core.Domain.Core.Enums
{
    public enum MessageStatus
    {
        /// <summary>
        /// Не отправленно
        /// </summary>
        Non,

        /// <summary>
        /// Отправленно
        /// </summary>
        Sent,

        /// <summary>
        /// Доставленно
        /// </summary>
        Delivered,

        /// <summary>
        /// Прочитано
        /// </summary>
        Read,

        /// <summary>
        /// ошибка отправки
        /// </summary>
        Error
    }
}