using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Isbn { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateOnly? PublishYear { get; set; }

    public string? Publisher { get; set; }

    public string Author { get; set; } = null!;

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
