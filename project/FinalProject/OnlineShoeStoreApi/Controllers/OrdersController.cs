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
//    public class OrdersController : ControllerBase
//    {
//        private readonly OnlineShoeStoreContext _context;

//        public OrdersController(OnlineShoeStoreContext context)
//        {
//            _context = context;
//        }

//        // Заказы пользователя по логину (доступно всем авторизованным)
//        [HttpGet("user/{login}")]
//        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(string login)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
//            if (user == null) return NotFound("Пользователь не найден");

//            var orders = await _context.Orders
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .Where(o => o.UserId == user.Id)
//                .ToListAsync();

//            return orders;
//        }

//        // Изменение статуса и даты доставки — только админ/менеджер
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Администратор,Менеджер")]
//        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderModel model)
//        {
//            var order = await _context.Orders.FindAsync(id);
//            if (order == null) return NotFound();

//            order.Status = model.Status;
//            order.DeliveryDate = model.DeliveryDate;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }

//    public class UpdateOrderModel
//    {
//        public string Status { get; set; } = string.Empty;
//        public DateTime DeliveryDate { get; set; }
//    }
//}