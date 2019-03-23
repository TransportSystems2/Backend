using System;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    [Flags]
    public enum OrderStatus
    {
        /// <summary>
        /// Новый
        /// </summary>
        New = 1,

        /// <summary>
        /// Принят
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// Готов для торговли
        /// </summary>
        ReadyForTrade = 4,

        /// <summary>
        /// Отправлен на торги
        /// </summary>
        SentToTrading = 8,

        /// <summary>
        /// Выдан диспетчеру
        /// </summary>
        AssignedDispatcher = 16,

        /// <summary>
        /// Выдан водителю
        /// </summary>
        AssignedDriver = 32,

        /// <summary>
        /// Подтвержден водителем
        /// </summary>
        ConfirmedByDriver = 64,

        /// <summary>
        /// На пути к клиенту
        /// </summary>
        WentToCustomer = 128,

        /// <summary>
        /// Прибыл на место погрузки
        /// </summary>
        ArrivedAtLoadingPlace = 256,

        /// <summary>
        /// Транспортное средство загруженно
        /// </summary>
        VehicleIsLoaded = 512,

        /// <summary>
        /// Транспортное средство доставлено
        /// </summary>
        VehicleIsDelivered = 1024,

        /// <summary>
        /// Оплата получена
        /// </summary>
        PaymentIsReceived = 2048,

        /// <summary>
        /// Выполнен
        /// </summary>
        Completed = 4096,

        /// <summary>
        /// Отменен
        /// </summary>
        Canceled = 8192,

        /// <summary>
        /// Выполняется
        /// </summary>
        IsCarriedOut = WentToCustomer | ArrivedAtLoadingPlace | VehicleIsLoaded | VehicleIsDelivered
    }
}