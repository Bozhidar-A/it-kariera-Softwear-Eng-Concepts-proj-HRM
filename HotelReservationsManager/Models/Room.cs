using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationsManager.Models
{
    public class Room
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int capacity { get; set; }
        //html select
        [Required]
        public string type { get; set; }
        [Required]
        public bool free { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal bedAdultPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal bedChildPrice { get; set; }
    }
}
