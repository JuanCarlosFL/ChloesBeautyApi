using System;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Appointment
    {
        #region Public Properties

        public int AppointmentId { get; set; }

        public virtual Availability Availability { get; set; }

        public int AvailabilityId { get; set; }

        public bool Deleted { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual Person Person { get; set; }

        public int PersonId { get; set; }

        public virtual Treatment Treatment { get; set; }

        public int TreatmentId { get; set; }

        #endregion Public Properties
    }
}