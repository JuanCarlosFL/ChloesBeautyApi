using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.ViewModels
{
    public class RecoverPasswordViewModel
    {
        #region Public Properties

        public string ConfirmPassword { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        #endregion Public Properties
    }
}