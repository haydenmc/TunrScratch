using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tunr.Models.BindingModels
{
    public class RegistrationRequestBindingModel
    {
        [JsonProperty("email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonProperty("password")]
        [MinLength(5)]
        public string Password { get; set; }
    }
}
