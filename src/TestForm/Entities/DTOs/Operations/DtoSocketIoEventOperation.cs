using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestForm.Enums;

namespace TestForm.Entities.DTOs.Operations
{
    public class DtoSocketIoEventOperation : DtoSocketIoEventBase
    {
        public Operation Operation { get; set; }
    }
}
