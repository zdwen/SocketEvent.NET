using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestForm.Entities.DTOs.Event
{
    public class DtoEventBase
    {
        public string ClientID { get; set; }
        public string RequestID { get; set; }
        public string BizEventName { get; set; }
    }
}
