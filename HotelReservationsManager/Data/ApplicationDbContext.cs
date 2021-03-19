using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelReservationsManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<HotelReservationsManager.Models.Client> Client { get; set; }
        public DbSet<HotelReservationsManager.Models.Room> Room { get; set; }
        public DbSet<HotelReservationsManager.Models.Reservation> Reservation { get; set; }
    }
}
