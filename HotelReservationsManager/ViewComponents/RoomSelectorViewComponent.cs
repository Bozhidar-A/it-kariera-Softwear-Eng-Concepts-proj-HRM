using HotelReservationsManager.Data;
using HotelReservationsManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelReservationsManager.ViewComponents
{
    public class RoomSelectorViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public RoomSelectorViewComponent(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page,
            int capacity,
            string type,
            bool bSearch)
        {
            ViewBag.type = _configuration.GetSection("RoomTypes").Get<List<string>>()
               .Select(x =>
               new SelectListItem() { Text = x.ToString(), Value = x.ToString() });

            IQueryable<Room> rooms;

            if (bSearch)//user is using the search
            {
                if (capacity == 0)//ints without value are binded to 0
                {
                    rooms = _context.Room.
                        Where(t => type == "Select Apartment Type" ? t.type.Contains("") : t.type == type)
                        .Where(f => f.free == true);
                }
                else//search by all paramaters
                {
                    rooms = _context.Room.
                        Where(c => c.capacity == capacity).
                        Where(t => t.type == "Select Apartment Type" ? t.type.Contains("") : t.type == type)
                        .Where(f => f.free == true);
                }
            }
            else//first load, load everything
            {
                rooms = _context.Room
                    .Where(f => f.free == true);
            }

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
    }
}
