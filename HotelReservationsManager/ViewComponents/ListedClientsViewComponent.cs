using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelReservationsManager.ViewComponents
{
    public class ListedClientsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ListedClientsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page, string firstName, string lastName, int whichVC)
        {
            //TempData["firstName"] = firstName;
            //TempData["lastName"] = lastName;

            IQueryable<Client> clients = _context.Client.
                Where(fn => fn.firstName.Contains(String.IsNullOrWhiteSpace(firstName) ? "" : firstName.Trim())).
                Where(ln => ln.lastName.Contains(String.IsNullOrWhiteSpace(lastName) ? "" : lastName.Trim())).
                Where(b => b.bCurrInReservation == false);


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

            return View((onePageOfProducts, whichVC));
        }
    }
}
