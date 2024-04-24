using CozyToGo.Data;
using CozyToGo.DTO.DishDTO;
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
    public class DishesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DishesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("restaurantId/{idRestaurant}")]
        public async Task<IActionResult> GetDishes(int? idRestaurant)
        {
            if (idRestaurant == null)
            {
                return BadRequest("Missing Parameters");
            }
            var dishes = await _context.Dishes
    .Where(d => d.IdRestaurant == idRestaurant)
    .Include(d => d.DishIngredients)
    .ThenInclude(di => di.Ingredient)
    .Select(d => new DishDTO
    {
        IdDish = d.IdDish,
        Name = d.Name,
        Description = d.Description,
        Image = d.Image,
        IsAvailable = d.IsAvailable,
        IdRestaurant = d.IdRestaurant,
        Ingredients = d.DishIngredients.Select(di => new IngredientsDTO
        {
            IdIngredient = di.IdIngredient,
            Name = di.Ingredient.Name,
            Price = di.Ingredient.Price
        }).ToArray(),
        Price = d.DishIngredients.Sum(di => di.Ingredient.Price)
    })
    .ToListAsync();
            return Ok(dishes);
        }

        [HttpGet("restaurantId/{idRestaurant}/DishId/{IdDish}")]
        public async Task<IActionResult> GetDish(int? idRestaurant, int? idDish)
        {
            if (idRestaurant == null || idDish == null)
            {
                return BadRequest("Missing Parameters");
            }
            var dish = await _context.Dishes
                .Where(d => d.IdDish == idDish && d.IdRestaurant == idRestaurant)
                .Select(d => new
                {
                    d.IdDish,
                    d.Name,
                    d.Price
                })
                .FirstOrDefaultAsync();
            if (dish == null)
            {
                return NotFound();
            }
            return Ok(dish);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> PostDish([FromBody] AddDishDTO newDish)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Owner")
            {
                return Unauthorized();
            }
            if (await _context.Dishes.AnyAsync(d => d.Name == newDish.Name && d.IdRestaurant == newDish.IdRestaurant))
            {
                return BadRequest("Dish already exist in this restaurant");
            }

            var dish = new Dish
            {
                IdRestaurant = newDish.IdRestaurant,
                Name = newDish.Name,
                Description = newDish.Description,

            };
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();

            decimal totalPrice = 0;
            foreach (var ingredientId in newDish.Ingredients)
            {
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                if (ingredient == null)
                {
                    return BadRequest($"Ingredient with id {ingredientId} not found");
                }
                totalPrice += ingredient.Price;
                var dishIngredient = new DishIngredient
                {
                    IdDish = dish.IdDish,
                    IdIngredient = ingredientId
                };
                await _context.DishIngredients.AddAsync(dishIngredient);

            }
            dish.Price = totalPrice;
            _context.Update(dish);
            await _context.SaveChangesAsync();

            var dishResponse = await _context.Dishes
                .Where(d => d.IdDish == dish.IdDish)
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .Select(d => new DishDTO
                {
                    IdDish = d.IdDish,
                    Name = d.Name,
                    Description = d.Description,
                    Image = d.Image,
                    IsAvailable = d.IsAvailable,
                    Price = d.Price,
                    IdRestaurant = d.IdRestaurant,
                    Ingredients = d.DishIngredients.Select(di => new IngredientsDTO
                    {
                        IdIngredient = di.IdIngredient,
                        Name = di.Ingredient.Name,
                        Price = di.Ingredient.Price
                    }).ToArray()
                }).FirstOrDefaultAsync();
            return Ok(dishResponse);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{idDish}/uploadImage")]
        public async Task<IActionResult> UploadDishImage(int? idDish, IFormFile file)
        {
            if (idDish == null)
            {
                return BadRequest("Missing Parameters");
            }
            var dish = await _context.Dishes.Include(d => d.Restaurant).FirstOrDefaultAsync(d => d.IdDish == idDish);
            if (dish == null)
            {
                return BadRequest("Dish Not Found");
            }
            // Ottieni l'ID dell'utente corrente
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Se l'utente è un Owner, verifica che stia cercando di caricare un'immagine per il proprio ristorante
            if (User.IsInRole("Owner") && dish.Restaurant.IdOwner.ToString() != userId)
            {
                return Unauthorized("You can only upload images for your own restaurant");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid File");
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot", "images", fileName);
            using (var stram = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stram);
            }
            dish.Image = fileName;
            _context.Update(dish);
            await _context.SaveChangesAsync();
            return Ok((new { idDish = dish.IdDish, image = fileName }));
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{idDish}")]
        public async Task<IActionResult> ToggleDish(int? idDish)
        {
            if (idDish == null)
            {
                return BadRequest("Missing parameters");

            }
            var dish = await _context.Dishes.FindAsync(idDish);
            if (dish == null)
            {
                return NotFound("Dish not found");
            }
            dish.IsAvailable = !dish.IsAvailable;
            _context.Update(dish);
            await _context.SaveChangesAsync();

            var dishResponse = await _context.Dishes
                .Where(d => d.IdDish == dish.IdDish)
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .Select(d => new DishDTO
                {
                    IdDish = d.IdDish,
                    Name = d.Name,
                    Description = d.Description,
                    Image = d.Image,
                    IsAvailable = d.IsAvailable,
                    Price = d.Price,
                    IdRestaurant = d.IdRestaurant,
                    Ingredients = d.DishIngredients.Select(di => new IngredientsDTO
                    {
                        IdIngredient = di.IdIngredient,
                        Name = di.Ingredient.Name,
                        Price = di.Ingredient.Price
                    }).ToArray()
                }).FirstOrDefaultAsync();
            return Ok(dishResponse);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("edit/{idDish}")]
        public async Task<IActionResult> UpdateDish(int? idDish, [FromBody] EditDishDTO editedDish)
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

            if (idDish == null)
            {
                return BadRequest("Missing parameters");
            }
            var dish = await _context.Dishes
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .FirstOrDefaultAsync(d => d.IdDish == idDish);
            if (dish == null)
            {
                return NotFound("Dish not found");
            }
            dish.Name = editedDish.Name;
            dish.Description = editedDish.Description;


            _context.DishIngredients.RemoveRange(dish.DishIngredients);

            decimal totalPrice = 0;
            foreach (var ingredientId in editedDish.Ingredients)
            {
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                if (ingredient == null)
                {
                    return BadRequest($"Ingredient with id {ingredientId} not found");
                }
                totalPrice += ingredient.Price;
                var dishIngredient = new DishIngredient
                {
                    IdDish = dish.IdDish,
                    IdIngredient = ingredientId
                };
                await _context.DishIngredients.AddAsync(dishIngredient);
            }
            dish.Price = totalPrice;
            _context.Update(dish);
            await _context.SaveChangesAsync();

            var dishResponse = await _context.Dishes
                .Where(d => d.IdDish == dish.IdDish)
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .Select(d => new DishDTO
                {
                    IdDish = d.IdDish,
                    Name = d.Name,
                    Description = d.Description,
                    Image = d.Image,
                    IsAvailable = d.IsAvailable,
                    Price = d.Price,
                    IdRestaurant = d.IdRestaurant,
                    Ingredients = d.DishIngredients.Select(di => new IngredientsDTO
                    {
                        IdIngredient = di.IdIngredient,
                        Name = di.Ingredient.Name,
                        Price = di.Ingredient.Price
                    }).ToArray()
                }).FirstOrDefaultAsync();
            return Ok(dishResponse);
        }

        [HttpGet("search/{dishName}")]
        public async Task<IActionResult> GetDishesByName(string dishName)
        {
            var dishes = await _context.Dishes
                .Where(d => d.Name.Contains(dishName.ToLower()))
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .Select(d => new DishDTO
                {
                    IdDish = d.IdDish,
                    Name = d.Name,
                    Description = d.Description,
                    Image = d.Image,
                    IsAvailable = d.IsAvailable,
                    Price = d.Price,
                    IdRestaurant = d.IdRestaurant,
                    Ingredients = d.DishIngredients.Select(di => new IngredientsDTO
                    {
                        IdIngredient = di.IdIngredient,
                        Name = di.Ingredient.Name,
                        Price = di.Ingredient.Price
                    }).ToArray()
                })
                .ToListAsync();
            if (dishes == null)
            {
                return NotFound("No dishes found");
            }
            return Ok(dishes);
        }
    }
}
