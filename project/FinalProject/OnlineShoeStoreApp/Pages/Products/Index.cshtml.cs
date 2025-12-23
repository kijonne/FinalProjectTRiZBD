using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
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

        public bool NoResults { get; set; } = false; // Добавили свойство

        public async Task OnGetAsync()
        {
            Manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();

            // Базовый запрос
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier);

            // Поиск по описанию (для MSSQL — Contains с игнором регистра)
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
    }
}
