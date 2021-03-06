using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChloesBeauty.API.ViewModels
{
    public class RegisterViewModel
    {
        #region Public Properties

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Surname { get; set; }

        #endregion Public Properties
    }
}