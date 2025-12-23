using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;

namespace OnlineShoeStoreApp.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly OnlineShoeStoreLibrary.Contexts.OnlineShoeStoreContext _context;

        public IndexModel(OnlineShoeStoreLibrary.Contexts.OnlineShoeStoreContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Orders = new List<Order>();
                return;
            }

            var userLogin = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userLogin);

            if (user == null)
            {
                Orders = new List<Order>();
                return;
            }

            var isAdminOrManager = User.IsInRole("Администратор") || User.IsInRole("Менеджер");

            if (isAdminOrManager)
            {
                Orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            else
            {
                Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == user.UserId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
        }
    }
}
