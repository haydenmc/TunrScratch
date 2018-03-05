using Newtonsoft.Json;

namespace Tunr.Models.BindingModels
{
    public class LoginRequestBindingModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
