using BookStore.Models;
using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class OrderDetail
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public decimal UnitPrice { get; set; }

    public int Amount { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
