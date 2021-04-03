using System;
using System.Threading;
using System.Threading.Tasks;
using HotelReservationsManager.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationsManager.Services
{
    public class UpdateEndedReservations : CronJobService
    {
        private readonly ILogger<UpdateEndedReservations> _logger;

        private readonly ApplicationDbContext _context;

        public UpdateEndedReservations(IScheduleConfig<UpdateEndedReservations> config, ILogger<UpdateEndedReservations> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;

            _context = serviceProvider.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateEndedReservations starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} UpdateEndedReservations is working.");

            //var us = _context.Users.Where(user => user.UserName == "admin").FirstOrDefault();

            //us.hireDate = DateTime.UtcNow;

            var reservations = _context.Reservation.Include(cl => cl.clients)
              .Include(u => u.applicationUser)
              .Include(r => r.room)
              .Where(d => d.reservationDate < DateTime.UtcNow)
              .ToList();

            if(reservations.Count != 0)
            {
                foreach (var reservation in reservations)
                {
                    foreach (var cl in reservation.clients)
                    {
                        cl.bCurrInReservation = false;
                    }

                    //room has been deleted from database
                    if (reservation.room != null)
                    {
                        reservation.room.free = true;
                    }
                }


                _context.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateEndedReservations is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
