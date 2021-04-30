using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla User que ha mapeado el ORM
    public partial class User
    {

        // Tiene relacionadas las tablas UsersRole y Person
        public User()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public int PersonId { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Person Person { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
