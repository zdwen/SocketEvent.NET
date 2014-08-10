using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestForm.Entities.DTOs.Event
{
    public class DtoEventEnqueue<T> : DtoEventBase
    {
        public T Data { get; set; }
    }
}
