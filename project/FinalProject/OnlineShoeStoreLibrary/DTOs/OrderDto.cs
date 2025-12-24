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
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int ReceiveCode { get; set; }
        public bool IsFinished { get; set; }
        public decimal TotalAmount { get; set; } // итоговая стоимость
        public string UserLogin { get; set; } = null!; // логин пользователя
    }
}
