using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TransportSystems.Backend.Identity.Signin.Models
{
    public class PhoneLoginViewModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
    }
}