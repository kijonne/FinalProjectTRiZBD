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

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if(!User.Identity.IsAuthenticated)
            {
                Order = new List<Order>();
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (user == null) return;

            var isAdminOrManager = User.IsInRole("admin") || User.IsInRole("manager");

            if (isAdminOrManager)
            {
                Order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            else
            {
                Order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == user.UserId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
        }
    }
}
