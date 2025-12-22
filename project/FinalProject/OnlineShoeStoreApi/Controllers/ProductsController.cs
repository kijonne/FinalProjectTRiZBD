//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineShoeStoreLibrary.Contexts;
//using OnlineShoeStoreLibrary.Models;

//namespace OnlineShoeStore.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class ProductsController : ControllerBase
//    {
//        private readonly OnlineShoeStoreContext _context;

//        public ProductsController(OnlineShoeStoreContext context)
//        {
//            _context = context;
//        }

//        // Все товары
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
//        {
//            return await _context.Products
//                .Include(p => p.Category)
//                .Include(p => p.Manufacturer)
//                .Include(p => p.Supplier)
//                .ToListAsync();
//        }

//        // Товар по артикулу
//        [HttpGet("{article}")]
//        public async Task<ActionResult<Product>> GetProduct(string article)
//        {
//            var product = await _context.Products
//                .Include(p => p.Category)
//                .Include(p => p.Manufacturer)
//                .Include(p => p.Supplier)
//                .FirstOrDefaultAsync(p => p.Article == article);

//            if (product == null) return NotFound();

//            return product;
//        }

//        // Добавление, обновление, удаление — только админ/менеджер
//        [HttpPost]
//        [Authorize(Roles = "Администратор,Менеджер")]
//        public async Task<ActionResult<Product>> PostProduct(Product product)
//        {
//            _context.Products.Add(product);
//            await _context.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetProduct), new { article = product.Article }, product);
//        }

//        [HttpPut("{article}")]
//        [Authorize(Roles = "Администратор,Менеджер")]
//        public async Task<IActionResult> PutProduct(string article, Product product)
//        {
//            if (article != product.Article) return BadRequest();

//            _context.Entry(product).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpDelete("{article}")]
//        [Authorize(Roles = "Администратор,Менеджер")]
//        public async Task<IActionResult> DeleteProduct(string article)
//        {
//            var product = await _context.Products.FindAsync(article);
//            if (product == null) return NotFound();

//            _context.Products.Remove(product);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}