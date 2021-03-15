using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Models
{
    public class Reservation
    {
        [Required]
        public int ID { get; set; }
        public Room room { get; set; }
        public ApplicationUser applicationUser { get; set; }
        [Required]
        public IList<Client> clients { get; set; }
        [Required]
        public DateTime reservationDate { get; set; }
        [Required]
        public DateTime releaseDate { get; set; }
        [Required]
        public bool breakfast { get; set; }
        [Required]
        public bool allInclusive { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal finalPrice { get; set; }
    }
}
