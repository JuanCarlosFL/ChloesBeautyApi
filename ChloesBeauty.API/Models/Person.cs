using System;
using System.Collections.Generic;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class Person
    {
        public Person()
        {
            Appointments = new HashSet<Appointment>();
            Users = new HashSet<User>();
        }

        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public int Points { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string ContactHow { get; set; }
        public bool Deleted { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
