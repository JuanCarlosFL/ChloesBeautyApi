using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.ViewModels
{
    public class AvailabilitiesViewModel
    {
        #region Public Properties

        public DateTime Date { get; set; }

        public IEnumerable<TimesByDate> TimesByDate { get; set; }

        #endregion Public Properties
    }
}