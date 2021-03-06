using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Treatment
    {
        public Treatment()
        {
            Appointments = new HashSet<Appointment>();
        }

        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public int? Points { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Loyalty Loyalty { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
