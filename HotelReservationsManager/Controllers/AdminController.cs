using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelReservationsManager.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? page, 
            string userName, 
            string email,
            string firstName,
            string middleName,
            string lastName)
        {
            TempData["userName"] = userName;
            TempData["email"] = email;
            TempData["firstName"] = firstName;
            TempData["middleName"] = middleName;
            TempData["lastName"] = lastName;

            IQueryable<ApplicationUser> products = _context.Users.
                Where(fn => fn.firstName.Contains(String.IsNullOrWhiteSpace(firstName) ? "" : firstName.Trim())).
                 Where(ln => ln.lastName.Contains(String.IsNullOrWhiteSpace(lastName) ? "" : lastName.Trim())).
                  Where(mn => mn.middleName.Contains(String.IsNullOrWhiteSpace(middleName) ? "" : middleName.Trim())).
                   Where(em => em.Email.Contains(String.IsNullOrWhiteSpace(email) ? "" : email.Trim())).
                    Where(un => un.UserName.Contains(String.IsNullOrWhiteSpace(userName) ? "" : userName.Trim()));


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


        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            return LocalRedirect("/Identity/Account/Register");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockOut(string? ID)
        {
            DateTime twoHundredYears = DateTime.Now;
            twoHundredYears = twoHundredYears.AddYears(200);
            //this should be enough
            var user = await _userManager.FindByIdAsync(ID);
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, twoHundredYears);
            return RedirectToAction("Index","Admin");
        }
    }
}
