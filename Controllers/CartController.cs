using CozyToGo.Data;
using CozyToGo.DTO.CartDTO;
using CozyToGo.DTO.OrderDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }





        [Authorize]
        [HttpPost("submitOrder")]
        public async Task<IActionResult> SubmitOrder([FromBody] CartDTO cart)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Creare un nuovo ordine
            var order = new Order
            {
                IdUser = Convert.ToInt32(userId),
                OrderDate = DateTime.Now,
                DeliveryAddress = cart.DeliveryAddress,
                City = cart.City,
                Notes = cart.Notes
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal totalOrder = 0;

            // Creare i dettagli dell'ordine per ogni piatto nel carrello
            List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();
            foreach (var dish in cart.Dishes)
            {
                var orderDetail = new OrderDetail
                {
                    IdOrder = order.IdOrder,
                    IdDish = dish.IdDish,
                    Quantity = dish.Quantity
                };
                _context.OrderDetails.Add(orderDetail);

                // Calcola il prezzo totale del piatto (prezzo degli ingredienti * quantità) e aggiungilo al totale dell'ordine
                var dishFromDb = await _context.Dishes
                    .Include(d => d.DishIngredients)
                    .ThenInclude(di => di.Ingredient)
                    .FirstOrDefaultAsync(d => d.IdDish == dish.IdDish);

                if (dishFromDb != null)
                {
                    var dishPrice = dishFromDb.DishIngredients.Sum(di => di.Ingredient.Price);
                    totalOrder += dishPrice * dish.Quantity;

                    // Aggiungi l'articolo alla lista degli articoli per la sessione di checkout
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = dishFromDb.Name,
                            },
                            UnitAmountDecimal = dishPrice * 100, // Converti il prezzo in centesimi
                        },
                        Quantity = dish.Quantity,
                    });
                }
            }

            // Imposta il totale dell'ordine
            order.Total = totalOrder;

            // Stripe
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "http://localhost:5173/",
                CancelUrl = "https://example.com/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            _context.Update(order);
            await _context.SaveChangesAsync();

            var orderDto = new OrderDTO
            {
                IdOrder = order.IdOrder,
                IdUser = order.IdUser,
                Total = order.Total,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                DeliveryAddress = order.DeliveryAddress,
                City = order.City,
                Notes = order.Notes,
            };

            // Restituisci l'ID della sessione al frontend
            return Ok(new { sessionId = session.Id });
        }
    }
}
