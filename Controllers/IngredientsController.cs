using CozyToGo.Data;
using CozyToGo.DTO.IngredientDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost]
        public async Task<IActionResult> PostIngredient([FromBody] AddIngredientDTO newIngredient)
        {
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
    }
}
