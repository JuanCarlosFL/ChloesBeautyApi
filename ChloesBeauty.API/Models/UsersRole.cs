using System;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla UsersRole que ha mapeado el ORM
    public partial class UsersRole
    {
        // Tiene relacionadas las tablas Role y User
        #region Public Properties

        public DateTime ModifiedDate { get; set; }

        public virtual Role Role { get; set; }

        public byte RoleId { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        #endregion Public Properties
    }
}