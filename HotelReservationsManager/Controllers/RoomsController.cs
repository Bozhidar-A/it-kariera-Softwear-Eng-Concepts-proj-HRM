using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservationsManager.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public RoomsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(int? page, 
            int capacity, 
            string type, 
            bool bFree,
            bool bSearch)
        {

            TempData["capacity"] = capacity;
            TempData["bFree"] = bFree;

            ViewBag.type = _configuration.GetSection("RoomTypes").Get<List<string>>()
                .Select(x => 
                new SelectListItem() { Text = x.ToString(), Value = x.ToString() });

            IQueryable<Room> rooms;

            if(bSearch)//user is using the search
            {
                if(capacity == 0)//ints without value are binded to 0
                {
                    rooms = _context.Room.
                        Where(t => type == "Select Apartment Type" ? t.type.Contains("") : t.type == type).
                        Where(bF => bF.free == bFree);
                }
                else//search by all paramaters
                {
                    rooms = _context.Room.
                        Where(c => c.capacity == capacity).
                        Where(t => type == "Select Apartment Type" ? t.type.Contains("") : t.type == type).
                        Where(bF => bF.free == bFree);
                }
            }
            else//first load, load everything
            {
                rooms = _context.Room;
            }

            //this is bad as it can take the whole db
            //todo optimise by rewriting

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
            var onePageOfRooms = rooms.ToPagedList(pageNumber, pageSize);

            return View(onePageOfRooms);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            ViewBag.type = _configuration.GetSection("RoomTypes").Get<List<string>>()
                .Select(x =>
                new SelectListItem() { Text = x.ToString(), Value = x.ToString() });

            return View(room);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.type = _configuration.GetSection("RoomTypes").Get<List<string>>()
                .Select(x =>
                new SelectListItem() { Text = x.ToString(), Value = x.ToString() });

            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ID,capacity,type,free,bedAdultPrice,bedChildPrice")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.free = true;
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,capacity,type,free,bedAdultPrice,bedChildPrice")] Room room)
        {
            if (id != room.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.ID))
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
            return View(room);
        }

        // GET: Rooms/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.
                Include(r => r.reservations).
                FirstAsync(m => m.ID == id);
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.ID == id);
        }
    }
}
