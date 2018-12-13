using System;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    [Flags]
    public enum OrderStatus
    {
        /// <summary>
        /// Новый
        /// </summary>
        New = 0,

        /// <summary>
        /// Принят
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// Готов для торговли
        /// </summary>
        ReadyForTrade = 2,

        /// <summary>
        /// Отправлен на торги
        /// </summary>
        SentToTrading = 4,

        /// <summary>
        /// Выдан диспетчеру
        /// </summary>
        AssignedDispatcher = 8,

        /// <summary>
        /// Выдан водителю
        /// </summary>
        AssignedDriver = 16,

        /// <summary>
        /// Подтвержден водителем
        /// </summary>
        ConfirmedByDriver = 32,

        /// <summary>
        /// На пути к клиенту
        /// </summary>
        WentToCustomer = 64,

        /// <summary>
        /// Прибыл на место погрузки
        /// </summary>
        ArrivedAtLoadingPlace = 128,

        /// <summary>
        /// Транспортное средство загруженно
        /// </summary>
        VehicleIsLoaded = 256,

        /// <summary>
        /// Транспортное средство доставлено
        /// </summary>
        VehicleIsDelivered = 512,

        /// <summary>
        /// Оплата получена
        /// </summary>
        PaymentIsReceived = 1024,

        /// <summary>
        /// Выполнен
        /// </summary>
        Completed = 2048,

        /// <summary>
        /// Отменен
        /// </summary>
        Canceled = 4096,

        /// <summary>
        /// Выполняется
        /// </summary>
        IsCarriedOut = WentToCustomer | ArrivedAtLoadingPlace | VehicleIsLoaded | VehicleIsDelivered
    }
}