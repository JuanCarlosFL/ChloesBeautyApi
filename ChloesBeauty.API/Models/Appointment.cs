using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Appointment
    {
        public int AppointmentId { get; set; }
        public int PersonId { get; set; }
        public int TreatmentId { get; set; }
        public int AvailabilityId { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Availability Availability { get; set; }
        public virtual Person Person { get; set; }
        public virtual Treatment Treatment { get; set; }
    }
}
