namespace TransportSystems.Backend.Application.Models.Geo
{
    public class AddressAM : BaseAM
    {
        /// <summary>
        /// Страна Россия
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Область Россия, Ярославская область
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// Район области Россия, Ярославская область, Рыбинский район
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Населённый пункт: город / поселок / деревня / село и т. п. Россия, Ярославская область, городской округ Рыбинск, г.Рыбинск
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Район города Россия, Москва, Северо-Восточный административный округ 
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        public string House { get; set; }

        public string Request { get; set; }

        public string FormattedText { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// При построении маршрута, координаты могут быть скорректированны. Храним их здесь.
        /// </summary>
        public double AdjustedLatitude { get; set; }

        public double AdjustedLongitude { get; set; }
    }
}