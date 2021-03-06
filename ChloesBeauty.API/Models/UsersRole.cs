using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class UsersRole
    {
        public int UserId { get; set; }
        public byte RoleId { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
