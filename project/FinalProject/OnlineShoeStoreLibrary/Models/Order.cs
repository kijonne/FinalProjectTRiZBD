using System;
using System.Collections.Generic;

namespace OnlineShoeStoreLibrary.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }

    public int ReceiveCode { get; set; }

    public bool IsFinished { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
