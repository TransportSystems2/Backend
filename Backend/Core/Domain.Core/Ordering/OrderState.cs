using System;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    public class OrderState : BaseEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }

        /// <summary>
        /// Время подачи
        /// </summary>
        public DateTime TimeOfDelivery { get; set; }          /// <summary>
        /// Биржа на которой торговался заказ
        /// </summary>         public int MarketId { get; set; }          /// <summary>
        /// Генеральный подрядчик
        /// </summary>         public int GenCompanyId { get; set; }          /// <summary>
        /// Субподрядчик
        /// </summary>         public int SubCompanyId { get; set; }          /// <summary>
        /// гараж субподрядчика
        /// </summary>         public int GarageId { get; set; }          /// <summary>
        /// Клиент
        /// </summary>         public int CustomerId { get; set; }

        /// <summary>
        /// Модератор создавший заказ
        /// </summary>         public int ModeratorId { get; set; } 
        /// <summary>
        /// Диспетчер принявший заказ
        /// </summary>
        public int DispatcherId { get; set; }          /// <summary>
        /// Водитель выполняющий заказ
        /// </summary>         public int DriverId { get; set; }          /// <summary>
        /// Транспортное средство выполняющее заказ
        /// </summary>         public int VehicleId { get; set; }          /// <summary>
        /// Пройденый путь эвакуатора
        /// </summary>         public int PathId { get; set; }          /// <summary>
        /// Маршрут заказа
        /// </summary>         public int RouteId { get; set; }          /// <summary>
        /// Груз
        /// </summary>         public int CargoId { get; set; }          /// <summary>
        /// Счет
        /// </summary>         public int BillId { get; set; }
    }
}