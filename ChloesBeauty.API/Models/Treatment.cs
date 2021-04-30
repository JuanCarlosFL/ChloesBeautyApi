using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla Treatment que ha mapeado el ORM
    public partial class Treatment
    {
        // Tiene relacionadas las tablas Appointments y Loyalties
        public Treatment()
        {
            Appointments = new HashSet<Appointment>();
            Loyalties = new HashSet<Loyalty>();
        }

        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public int? Points { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Loyalty> Loyalties { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
