using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoeStoreLibrary.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Article { get; set; } = null!;

    public int Price { get; set; }

    public byte Discount { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public byte? Size { get; set; }

    public string? Color { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public bool Gender { get; set; }

    public string? PhotoName { get; set; }

    public string Unit { get; set; } = null!;

    [NotMapped]
    public string PhotoPath { get; set; } = string.Empty;

    [NotMapped]
    public decimal DiscountedPrice { get; set; }
    [NotMapped]
    public bool HasBigDiscount => Discount > 15;

    [NotMapped]
    public bool IsOutOfStock => Quantity == 0;

    [NotMapped]
    public bool HasDiscount => Discount > 0;

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;
}
