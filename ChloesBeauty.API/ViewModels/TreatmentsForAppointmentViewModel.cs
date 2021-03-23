using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.ViewModels
{
    public class TreatmentsForAppointmentViewModel
    {
        #region Public Properties

        public int Duration { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? Points { get; set; }

        public decimal Price { get; set; }

        #endregion Public Properties
    }
}