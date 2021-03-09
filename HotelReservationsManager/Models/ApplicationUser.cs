using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public int EGN { get; set; }
        public DateTime hireDate { get; set; }
        public bool isActive { get; set; }
        public DateTime? fireDate { get; set; }
    }
}
