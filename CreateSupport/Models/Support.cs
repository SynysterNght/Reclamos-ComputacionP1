﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CreateSupport.Models
{
    using Newtonsoft.Json;
    public class Support
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("clientid")]
        public int ClientId { get; set; }

        [JsonProperty("type")]
        public string Type{ get; set; }

        [JsonProperty("subject")]
        public string Subject{ get; set; }

        [JsonProperty("description")]
        public string Description{ get; set; }
        [JsonProperty("status")]
        public string Status{ get; set; }

        [JsonProperty("initialdate")]
        public string InitialDate{ get; set; }
    }
}