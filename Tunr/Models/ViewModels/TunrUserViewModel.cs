using System;
using Newtonsoft.Json;

namespace Tunr.Models.ViewModels
{
    public class TunrUserViewModel
    {
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
