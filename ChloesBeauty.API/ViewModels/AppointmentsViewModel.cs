using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.ViewModels
{
    public class AppointmentsViewModel
    {
        public DateTime AppointmentDate { get; set; }

        public string TreatmentName { get; set; }

        public string PersonName { get; set; }
    }
}
