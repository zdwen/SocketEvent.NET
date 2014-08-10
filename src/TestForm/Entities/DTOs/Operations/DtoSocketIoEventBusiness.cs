using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestForm.Entities.DTOs.Business;
using TestForm.Enums;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace TestForm.Entities.DTOs.Operations
{
    public class DtoSocketIoEventBusiness<T> : DtoSocketIoEventBase
        where T : DtoBusiness
    {
        public T Data { get; set; }
    }
}
