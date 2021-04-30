﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla Role que ha mapeado el ORM
    public partial class Role
    {
        // Tiene relacionada la tabla UsersRole
        public Role()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public byte RoleId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
