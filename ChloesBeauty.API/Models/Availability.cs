using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    // Tabla  que ha mapeado el ORM
    public partial class Availability
    {
        // Tiene relacionada la tabla Appointments
        public Availability()
        {
            Appointments = new HashSet<Appointment>();
        }

        public int AvailabilityId { get; set; }
        public DateTime Date { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
