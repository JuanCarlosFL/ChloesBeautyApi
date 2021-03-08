using System;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Loyalty
    {
        #region Public Properties

        public bool Deleted { get; set; }

        public int LoyaltyId { get; set; }

        public virtual Treatment LoyaltyNavigation { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int Points { get; set; }

        public virtual Treatment Treatment { get; set; }

        public int TreatmentId { get; set; }

        #endregion Public Properties
    }
}