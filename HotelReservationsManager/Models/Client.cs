using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Models
{
    public class Client
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        [Phone]
        public string phoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public bool isAdult { get; set; }
    }
}
