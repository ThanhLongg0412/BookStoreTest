using System;
using System.Collections.Generic;

namespace BookStore.Models;

public class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }
}
