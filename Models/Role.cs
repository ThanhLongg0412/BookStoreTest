using System;
using System.Collections.Generic;

namespace bookstore.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
