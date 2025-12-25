using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Models;

namespace OnlineShoeStoreApp.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly OnlineShoeStoreLibrary.Contexts.OnlineShoeStoreContext _context;

        public IndexModel(OnlineShoeStoreLibrary.Contexts.OnlineShoeStoreContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = new List<Product>();
        public List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();

        [BindProperty(SupportsGet = true)]
        public string? SearchDescription { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ManufacturerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OnlyDiscount { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OnlyInStock { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "article_asc";

        public bool NoResults { get; set; } = false;

        public async Task OnGetAsync()
        {
            Manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();

            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier);

            // Поиск по описанию
            if (!string.IsNullOrWhiteSpace(SearchDescription))
            {
                var search = SearchDescription.Trim();
                query = query.Where(p => p.Description != null &&
                                         EF.Functions.Like(p.Description, $"%{search}%"));
            }

            // Фильтр по производителю
            if (ManufacturerId.HasValue && ManufacturerId.Value > 0)
            {
                query = query.Where(p => p.ManufacturerId == ManufacturerId.Value);
            }

            // Фильтр по максимальной цене
            if (MaxPrice.HasValue && MaxPrice.Value > 0)
            {
                query = query.Where(p => p.Price <= MaxPrice.Value);
            }

            // Только со скидкой
            if (OnlyDiscount)
            {
                query = query.Where(p => p.Discount > 0);
            }

            // Только в наличии
            if (OnlyInStock)
            {
                query = query.Where(p => p.Quantity > 0);
            }

            // Сортировка
            IOrderedQueryable<Product> orderedQuery = SortOrder switch
            {
                "supplier_asc" => query.OrderBy(p => p.Supplier!.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Article)
            };

            // Выполняем запрос
            Product = await orderedQuery.AsNoTracking().ToListAsync();

            // Оповещение об отсутствии результатов
            NoResults = !Product.Any();
        }

        public async Task<IActionResult> OnPostOrderAsync(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.Quantity <= 0)
            {
                TempData["Error"] = "Товар недоступен для заказа";
                return RedirectToPage();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (user == null) return Unauthorized();

            var order = new Order
            {
                UserId = user.UserId,
                OrderDate = DateTime.Now.Date,
                DeliveryDate = DateTime.Now.AddDays(7),
                ReceiveCode = new Random().Next(100, 1000),
                IsFinished = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = productId,
                Quantity = 1
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Заказ успешно создан!";
            return RedirectToPage();
        }
    }
}
