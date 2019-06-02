using System;
using Domain.Core;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    public class OrderState : CloneableEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }

        /// <summary>
        /// Время подачи
        /// </summary>
        public DateTime TimeOfDelivery { get; set; }
        
        /// <summary>
        /// Биржа на которой торговался заказ
        /// </summary>
        public int MarketId { get; set; }
        
        /// <summary>
        /// Генеральный подрядчик
        /// </summary>
        public int GenCompanyId { get; set; }
        
        /// <summary>
        /// Субподрядчик
        /// </summary>
        public int SubCompanyId { get; set; }
        
        /// <summary>
        /// гараж субподрядчика
        /// </summary>
        public int GarageId { get; set; }
        
        /// <summary>
        /// Клиент
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Диспетчер создавший заказ
        /// </summary>
        public int GenDispatcherId { get; set; }
        
        /// <summary>
        /// Диспетчер выполняющий заказ
        /// </summary>
        public int SubDispatcherId { get; set; }
        
        /// <summary>
        /// Водитель выполняющий заказ
        /// </summary>
        public int DriverId { get; set; }
        
        /// <summary>
        /// Транспортное средство выполняющее заказ
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Пройденый путь эвакуатора
        /// </summary>
        public int PathId { get; set; }
        
        /// <summary>
        /// Маршрут заказа
        /// </summary>
        public int RouteId { get; set; }
        
        /// <summary>
        /// Груз
        /// </summary>
        public int CargoId { get; set; }
        
        /// <summary>
        /// Счет
        /// </summary>
        public int BillId { get; set; }

        public override object Clone()
        {
            var result = new OrderState
            {
                OrderId = OrderId,
                Status = Status,
                TimeOfDelivery = TimeOfDelivery,
                MarketId = MarketId,
                GenCompanyId = GenCompanyId,
                SubCompanyId = SubCompanyId,
                GarageId = GarageId,
                CustomerId = CustomerId,
                GenDispatcherId = GenDispatcherId,
                SubDispatcherId = SubDispatcherId,
                DriverId = DriverId,
                VehicleId = VehicleId,
                PathId = PathId,
                RouteId = RouteId,
                CargoId = CargoId,
                BillId = BillId
            };

            return result;
        }
    }
}