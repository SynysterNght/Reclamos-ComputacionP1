using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaveToCosmossyn.Models
{
    public class iot
    {
        [JsonProperty("messageId")]
        public string messageId { get; set; }

        [JsonProperty("deviceId")]
        public string deviceId { get; set; }

        [JsonProperty("temperature")]
        public double temperature { get; set; }

        [JsonProperty("humidity")]
        public double humidity { get; set; }
    }
}
