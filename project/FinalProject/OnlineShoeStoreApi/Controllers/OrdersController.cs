
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;

namespace OnlineShoeStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineShoeStoreContext _context;

        public OrdersController(OnlineShoeStoreContext context)
        {
            _context = context;
        }

        // GET: api/orders/user/{login}
        [HttpGet("user/{login}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserLogin(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.UserId)
                .ToListAsync();

            return Ok(orders);
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            order.IsFinished = dto.IsFinished;
            order.DeliveryDate = dto.DeliveryDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO для обновления статуса заказа
    public class UpdateOrderStatusDto
    {
        public bool IsFinished { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}