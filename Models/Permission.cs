using System;
using System.Collections.Generic;

namespace bookstore.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
