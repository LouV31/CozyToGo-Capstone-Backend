using CozyToGo.Data;
using CozyToGo.DTO.OrderDTO;
using CozyToGo.DTO.RestaurantDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var orders = await _context.Orders
    .Where(o => o.IdUser.ToString() == userId)
    .Select(o => new OrderDTO
    {
        IdOrder = o.IdOrder,
        IdUser = Convert.ToInt32(userId),
        Total = o.Total,
        IsDelivered = o.IsDelivered,
        OrderDate = o.OrderDate,
        DeliveryDate = o.DeliveryDate,
        DeliveryAddress = o.DeliveryAddress,
        City = o.City,
        Notes = o.Notes,
        Restaurants = o.OrderDetails
            .GroupBy(d => d.Dish.Restaurant)
            .Select(g => new RestaurantForOrdersDTO
            {
                IdRestaurant = g.Key.IdRestaurant,
                Name = g.Key.Name,
                Dishes = g.Select(d => new OrderDetailDTO
                {
                    IdOrder = d.IdOrder,
                    IdDish = d.IdDish,
                    Name = d.Dish.Name,
                    Quantity = d.Quantity,
                    Image = d.Dish.Image,
                }).ToList()
            }).ToList()
    }).ToListAsync();
            return Ok(orders);
        }
    }
}
