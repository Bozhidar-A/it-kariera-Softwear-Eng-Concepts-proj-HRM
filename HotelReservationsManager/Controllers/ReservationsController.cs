using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HotelReservationsManager.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        //Note. No prices were specified for this in the doc. Making numbers up
        private const decimal breakfastPrice = 20m;

        private const decimal allInclusivePrice = 50m;

        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(int? page)
        {
            IQueryable<Reservation> products = _context.Reservation;

            int pageSize = 0; //default is 10

            if (!int.TryParse(Request.Cookies["pageSize"], out pageSize))
            {
                //bad cookie
                //set to default
                pageSize = 10;

                //make cookie
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.MaxValue;
                HttpContext.Response.Cookies.Append("pageSize", pageSize.ToString(), option);

                //TryParse will set to value of cookie if valid int
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            if (pageNumber == 0)
            {
                pageNumber = 1;// ?? catches null not 0
            }
            var onePageOfProducts = products.ToPagedList(pageNumber, pageSize);

            return View(onePageOfProducts);
        }

        public IActionResult OnGetClientReservationInputPartial(int whichVC)
        {
            return PartialView("_ClientReservationInputPartial", whichVC);
        }

        public IActionResult ReloadListedClientViewComp(string firstName, string lastName, int whichVC, int? page)
        {
            return ViewComponent("ListedClients", new { firstName = firstName, lastName = lastName, whichVC = whichVC, page = page });
        }

        public IActionResult ReloadRoomSelectorViewComp(int? page, int capacity, string type, bool bSearch)
        {
            return ViewComponent("RoomSelector", new { page = page, capacity = capacity, type = type, bSearch = bSearch });
        }

        [NonAction]
        public int NumberOfNights(DateTime date1, DateTime date2)
        {
            var frm = date1 < date2 ? date1 : date2;
            var to = date1 < date2 ? date2 : date1;
            var totalDays = (int)(to - frm).TotalDays;
            return totalDays > 1 ? totalDays : 1;
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(cl => cl.clients)
                .Include(u => u.applicationUser)
                .Include(r => r.room)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,reservationDate,releaseDate,breakfast,allInclusive,finalPrice,clients,room")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (reservation.clients.Count != reservation.clients.Distinct().Count())
                {
                    return View(reservation);
                    //user has inputed same user more then once
                }
                //todo move this to attrebute^

                //add user who made reservation
                reservation.applicationUser = await _userManager.GetUserAsync(User);

                //get clients with ef 
                for (int i = 0; i < reservation.clients.Count; i++)
                {
                    reservation.clients[i] = _context.Client.Where(cl => cl.ID == reservation.clients[i].ID).First();
                    reservation.clients[i].bCurrInReservation = true;
                }

                //get room with ef
                reservation.room = _context.Room.Where(r => r.ID == reservation.room.ID).First();
                reservation.room.free = false;

                //calculate price
                for(int i = 0; i < NumberOfNights(reservation.reservationDate, reservation.releaseDate); i++)
                {
                    //for over the nights the users will be stayiing
                    foreach(Client cl in reservation.clients)
                    {
                        //add the appropriate price
                        if (cl.isAdult)
                        {
                            reservation.finalPrice += reservation.room.bedAdultPrice;
                        }
                        else
                        {
                            reservation.finalPrice += reservation.room.bedChildPrice;
                        }
                    }
                }

                //add price for bnuses if needed
                if(reservation.breakfast)
                {
                    reservation.finalPrice += breakfastPrice;
                }

                if(reservation.allInclusive)
                {
                    reservation.finalPrice += allInclusivePrice;
                }

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            await _context.Entry(reservation).Collection(cl => cl.clients).LoadAsync();
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,reservationDate,releaseDate,breakfast,allInclusive,finalPrice")] Reservation reservation)
        {
            if (id != reservation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.ID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var reservation = await _context.Reservation.FindAsync(id);
            //await _context.Entry(reservation).Collection(cl => cl.clients).LoadAsync();
            //await _context.Entry(reservation).Property(r => r.room).LoadAsync();
            var reservation = await _context.Reservation.Include(cl => cl.clients)
                .Include(u => u.applicationUser)
                .Include(r => r.room)
                .FirstOrDefaultAsync(m => m.ID == id);
            foreach (var cl in reservation.clients)
            {
                cl.bCurrInReservation = false;
            }
            reservation.room.free = true;
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.ID == id);
        }
    }
}
