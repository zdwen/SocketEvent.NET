﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SocketEvent.Dto
{
    public class SocketEventResponseDto
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("error")]
        public ErrorDto Error { get; set; }
    }
}
