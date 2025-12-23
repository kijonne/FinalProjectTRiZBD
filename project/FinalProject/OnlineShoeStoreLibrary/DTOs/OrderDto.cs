using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoeStoreLibrary.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateOnly? OrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public bool IsFinished { get; set; }
    }
}
