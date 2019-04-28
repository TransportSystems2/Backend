using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TransportSystems.Backend.Identity.Signin.Models
{
    /// <summary>
    /// Модель телефона пользователя
    /// </summary>
    public class PhoneLoginViewModel
    {
        /// <summary>
        /// Номер телефона.
        /// </summary>
        /// <example>7XXXXXXXXXX</example>
        [Required]
        [DataType(DataType.PhoneNumber)]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
    }
}