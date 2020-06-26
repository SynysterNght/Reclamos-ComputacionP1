using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SendCloudToDevice
{
    public class mensajesyn
    {
        [JsonProperty("mensaje")]
        public string mensajeiot { get; set; }
    }
}
