using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Loyalty
    {
        public int LoyaltyId { get; set; }
        public int TreatmentId { get; set; }
        public int Points { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Treatment LoyaltyNavigation { get; set; }
    }
}
