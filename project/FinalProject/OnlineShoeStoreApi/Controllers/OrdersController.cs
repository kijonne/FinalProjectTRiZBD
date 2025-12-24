
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using OnlineShoeStoreLibrary.Models;

namespace OnlineShoeStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(OnlineShoeStoreContext context) : ControllerBase
    {
        private readonly OnlineShoeStoreContext _context = context;
        private DateTime todayDate = DateTime.FromDateTime(DateTime.Now);

        // GET: api/orders/user/{login}
        [HttpGet("user/{login}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserLogin(string login)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.User.Login == login)
                .ToListAsync();

            return orders is null ?
                NotFound() :
                Ok(orders.ToDtos());
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromQuery] DateTime? deliveryDate = null, [FromQuery] bool isFinished = false)
        {
            var order = await _context.Orders.FindAsync(id); // поиск заказа по id

            if (order is null)
                return NotFound();

            if (deliveryDate is null)
                order.DeliveryDate = todayDate;
            else
                order.DeliveryDate = (DateTime)deliveryDate;

            order.IsFinished = isFinished;

            try
            {
                await _context.SaveChangesAsync(); // Сохранение изменений в контексте
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return order.ToDto();
        }


    }
}