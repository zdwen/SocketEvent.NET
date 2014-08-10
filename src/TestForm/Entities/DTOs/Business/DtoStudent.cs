using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestForm.Entities.DTOs.Business
{
    public class DtoStudent : DtoBusiness
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
