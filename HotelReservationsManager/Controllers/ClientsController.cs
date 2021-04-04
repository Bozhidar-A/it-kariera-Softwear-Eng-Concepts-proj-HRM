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
using Microsoft.AspNetCore.Authorization;

namespace HotelReservationsManager.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? page, string firstName, string lastName)
        {
            TempData["firstName"] = firstName;
            TempData["lastName"] = lastName;

            IQueryable<Client> clients = _context.Client.
                Where(fn => fn.firstName.Contains(String.IsNullOrWhiteSpace(firstName) ? "" : firstName.Trim())).
                Where(ln => ln.lastName.Contains(String.IsNullOrWhiteSpace(lastName) ? "" : lastName.Trim()));


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
            var onePageOfProducts = clients.ToPagedList(pageNumber, pageSize);

            return View(onePageOfProducts);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(r => r.reservations)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,firstName,lastName,phoneNumber,email,isAdult")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,firstName,lastName,phoneNumber,email,isAdult")] Client client)
        {
            if (id != client.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ID))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client
                .Include(r => r.reservations)
                .FirstAsync(c => c.ID == id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ID == id);
        }
    }
}
