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
                    isDelivered = d.isDelivered
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
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    DeliveryAddress = o.DeliveryAddress,
                    City = o.City,
                    Notes = o.Notes,
                    OrderDetails = o.OrderDetails
    .Where(d => d.Dish.Restaurant.IdOwner.ToString() == userId)
    .Select(d => new
    {
        IdOrderDetail = d.IdOrderDetail,
        IsDelivered = d.isDelivered,
        Dish = new
        {
            IdDish = d.Dish.IdDish,
            Name = d.Dish.Name,
            Quantity = d.Quantity,
            Image = d.Dish.Image,
            Price = d.Dish.DishIngredients.Sum(d => d.Ingredient.Price),
        }
    }).ToList()


                }).ToListAsync();

            return Ok(orders);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("search/{userEmail}")]
        public async Task<IActionResult> GetOrderByEmail(string userEmail)
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
                .Where(o => o.OrderDetails.Any(d => d.Dish.Restaurant.IdOwner.ToString() == userId && o.User.Email.Contains(userEmail)))
                .Select(o => new
                {
                    IdOrder = o.IdOrder,
                    User = new
                    {

                        IdUser = o.User.IdUser,
                        Email = o.User.Email,
                    },
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    DeliveryAddress = o.DeliveryAddress,
                    City = o.City,
                    Notes = o.Notes,
                    OrderDetails = o.OrderDetails
    .Where(d => d.Dish.Restaurant.IdOwner.ToString() == userId)
    .Select(d => new
    {
        IdOrderDetail = d.IdOrderDetail,
        IsDelivered = d.isDelivered,
        Dish = new
        {
            IdDish = d.Dish.IdDish,
            Name = d.Dish.Name,
            Quantity = d.Quantity,
            Image = d.Dish.Image,
            Price = d.Dish.DishIngredients.Sum(d => d.Ingredient.Price),
        }
    }).ToList()


                }).ToListAsync();

            return Ok(orders);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("deliverOrderDetails")]
        public async Task<IActionResult> SetOrderDetailsDelivered([FromBody] List<int> orderDetailIds)
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
            if (orderDetailIds == null || orderDetailIds.Count == 0)
            {
                return BadRequest("Missing parameters");
            }
            var orderDetails = await _context.OrderDetails
                .Include(d => d.Dish)
                .ThenInclude(r => r.Restaurant)
                .Where(d => orderDetailIds.Contains(d.IdOrderDetail))
                .ToListAsync();
            if (orderDetails == null || orderDetails.Count == 0)
            {
                return NotFound("Order details not found");
            }
            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.Dish.Restaurant.IdOwner.ToString() != ownerId)
                {
                    return Unauthorized("You can only deliver your own dishes");
                }
                orderDetail.isDelivered = true;
                _context.Update(orderDetail);
            }

            // Check if all order details are delivered
            var orderIds = orderDetails.Select(d => d.IdOrder).Distinct();
            foreach (var orderId in orderIds)
            {
                var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.IdOrder == orderId);
                if (order.OrderDetails.All(d => d.isDelivered))
                {
                    order.DeliveryDate = DateTime.Now;
                    _context.Update(order);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(orderDetails.Select(d => new { idOrder = d.IdOrder, idOrderDetail = d.IdOrderDetail, isDelivered = d.isDelivered }));
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("sales")]
        public async Task<IActionResult> GetSales()
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
            var orderDetails = await _context.OrderDetails
    .Include(d => d.Dish)
    .ThenInclude(r => r.Restaurant)
    .Where(d => d.Dish.Restaurant.IdOwner.ToString() == ownerId && d.isDelivered == true)
    .Select(d => new
    {
        OrderDate = d.Order.OrderDate.Date,
        Total = d.Quantity * d.Dish.DishIngredients.Sum(di => di.Ingredient.Price)
    })
    .ToListAsync();

            var sales = orderDetails
                .GroupBy(d => d.OrderDate)
                .Select(g => new
                {
                    OrderDate = g.Key,
                    Total = g.Sum(d => d.Total)
                })
                .ToList();

            return Ok(sales);

        }
        [Authorize(Roles = "Owner")]
        [HttpGet("sales/{orderDate}")]
        public async Task<IActionResult> GetSalesByDate(DateOnly orderDate)
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
            var orderDetails = await _context.OrderDetails
                .Include(d => d.Dish)
                .ThenInclude(r => r.Restaurant)
                .Where(d => d.Dish.Restaurant.IdOwner.ToString() == ownerId && d.isDelivered == true && d.Order.OrderDate.Date == new DateTime(orderDate.Year, orderDate.Month, orderDate.Day))
                .Select(d => new
                {
                    Total = d.Quantity * d.Dish.DishIngredients.Sum(di => di.Ingredient.Price),
                    OrderDate = d.Order.OrderDate,
                }).ToListAsync();

            var totalSales = orderDetails.Sum(d => d.Total);

            var sales = new[]
    {
        new
        {
            OrderDate = new DateTime(orderDate.Year, orderDate.Month, orderDate.Day),
            Total = totalSales
        }
    };

            return Ok(sales);
        }

    }
}
