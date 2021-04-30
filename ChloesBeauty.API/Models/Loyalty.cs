using System;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla Loyalty que ha mapeado el ORM
    public partial class Loyalty
    {
        // Tiene relacionanada la tabla Treatment
        #region Public Properties

        public bool Deleted { get; set; }

        public int LoyaltyId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int Points { get; set; }

        public virtual Treatment Treatment { get; set; }

        public int TreatmentId { get; set; }

        #endregion Public Properties
    }
}