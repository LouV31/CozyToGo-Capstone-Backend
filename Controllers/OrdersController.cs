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
        [Authorize(Roles = "Owner")]
        [HttpGet("restaurantOrders")]
        public async Task<IActionResult> GetOrderByRestaurant()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("You must be logged to call this function");
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized("You must be a Owner to call this function");
            }
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Dish)
                .ThenInclude(r => r.Restaurant)
                .Include(o => o.User)
                .Where(o => o.OrderDetails.Any(d => d.Dish.Restaurant.IdOwner.ToString() == userId))
                .Select(o => new
                {
                    IdOrder = o.IdOrder,
                    User = new
                    {

                        IdUser = o.User.IdUser,
                        Email = o.User.Email,
                    },
                    Total = o.Total,
                    IsDelivered = o.IsDelivered,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    DeliveryAddress = o.DeliveryAddress,
                    City = o.City,
                    Notes = o.Notes,

                    Dishes = o.OrderDetails.Select(d => new
                    {

                        IdDish = d.IdDish,
                        Name = d.Dish.Name,
                        Quantity = d.Quantity,
                        Image = d.Dish.Image,
                    }).ToList()

                }).ToListAsync();

            return Ok(orders);
        }
        [Authorize(Roles = "Owner")]
        [HttpPut("{orderId}")]
        public async Task<IActionResult> SetOrderDelivered(int? orderId)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized("You must be logged to call this function");
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized("You must be a Owner to call this function");
            }
            if (orderId == null)
            {
                return BadRequest("Missing parameters");
            }
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            order.IsDelivered = true;
            order.DeliveryDate = DateTime.Now;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Ok(new { idOrder = order.IdOrder, isDelivered = order.IsDelivered, deliveryDate = order.DeliveryDate });
        }
    }
}
