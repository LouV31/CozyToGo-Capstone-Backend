using CozyToGo.Data;
using CozyToGo.DTO.IngredientDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get all ingredients from a business
        [Authorize(Roles = "Owner")]
        [HttpGet("{idBusiness}")]
        public async Task<IActionResult> GetIngredients(int? idBusiness)
        {
            if (idBusiness == null)
            {
                return BadRequest("Missing parameters");
            }
            var ingredients = await _context.Ingredients
                .Where(i => i.IdRestaurant == idBusiness)
                .Select(i => new
                {
                    i.IdIngredient,
                    i.Name,
                    i.Price
                })
                .ToListAsync();

            return Ok(ingredients);
        }

        // Method to get a specific ingredient from a business
        [HttpGet("{idBusiness}/{idIngredient}")]
        public async Task<IActionResult> GetIngredient(int? idIngredient, int? idBusiness)
        {
            if (idIngredient == null || idBusiness == null)
            {
                return BadRequest("Missing parameters");
            }
            var ingredient = await _context.Ingredients
                .Where(i => i.IdIngredient == idIngredient && i.IdRestaurant == idBusiness)
                .Select(i => new
                {
                    i.IdIngredient,
                    i.Name,
                    i.Price
                })
                .FirstOrDefaultAsync();
            if (ingredient == null)
            {
                return NotFound("Ingredient not found");
            }
            return Ok(ingredient);
        }

        // Method to add a new ingredient to a business
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> PostIngredient([FromBody] AddIngredientDTO newIngredient)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized("You must be logged to use this function");
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized("You must be a restaurant owner to use this function");
            }

            if (await _context.Ingredients.AnyAsync(i => i.Name == newIngredient.Name && i.IdRestaurant == newIngredient.IdRestaurant))
            {
                return BadRequest("Ingredient already exist");
            }
            var ingredient = new Ingredient
            {
                Name = newIngredient.Name,
                Price = newIngredient.Price,
                IdRestaurant = newIngredient.IdRestaurant
            };
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
            var ingredientDTO = new IngredientsDTO
            {
                IdIngredient = ingredient.IdIngredient,
                Name = ingredient.Name,
                Price = ingredient.Price
            };
            return Ok(ingredientDTO);
        }
        [Authorize(Roles = "Owner")]
        [HttpPut("{idIngredient}")]
        public async Task<IActionResult> PutIngredient(int? idIngredient, [FromBody] EditIngredientDTO ingredientDTO)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized("You must be logged to use this function");
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized("You must be a restaurant owner to use this function");
            }

            if (idIngredient == null)
            {
                return BadRequest("Missing parameters");
            }

            var ingredient = await _context.Ingredients.FindAsync(idIngredient);
            if (ingredient == null)
            {
                return NotFound("Ingredient not found");
            }

            ingredient.Name = ingredientDTO.Name;
            ingredient.Price = ingredientDTO.Price;

            _context.Update(ingredient);
            await _context.SaveChangesAsync();
            return Ok(new { idIngredient = ingredient.IdIngredient, name = ingredient.Name, price = ingredient.Price });
        }
        [Authorize(Roles = "Owner")]
        [HttpGet("search/{ingredientName}")]
        public async Task<IActionResult> GetIngredientsByName(string ingredientName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("You must be logget to use this function");
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized("You must be a restaurant owner to use this function");
            }
            var ingredients = await _context.Ingredients
               .Include(i => i.Restaurant)
                .Where(i => i.Name.Contains(ingredientName.ToLower()) && i.Restaurant.IdOwner == Convert.ToInt32(userId))
                .Select(i => new
                {
                    i.IdIngredient,
                    i.Name,
                    i.Price
                })
                .ToListAsync();
            if (ingredients == null)
            {
                return NotFound("No ingredients found");
            }
            return Ok(ingredients);
        }
    }
}
