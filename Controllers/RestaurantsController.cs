using CozyToGo.Data;
using CozyToGo.DTO.RestaurantDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Method to get all restaurants
        [HttpGet]
        public async Task<IActionResult> GetRestaurants()
        {
            var restaurants = await _context.Restaurants
                .Select(r => new RestaurantDTO
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Address = r.Address,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    IsActive = r.IsActive,
                    Image = r.Image
                })
                .ToListAsync();


            return Ok(restaurants);
        }

        // Method to get a specific restaurant
        [HttpGet("{idRestaurant}")]
        public async Task<IActionResult> GetRestaurant(int idRestaurant)
        {

            var restaurant = await _context.Restaurants
                .Where(r => r.IdRestaurant == idRestaurant)
                .Select(r => new
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Address = r.Address,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    IsActive = r.IsActive,
                    Image = r.Image
                })
                .FirstOrDefaultAsync();
            return Ok(restaurant);
        }

        // Metod to add a new restaurant
        [HttpPost]
        public async Task<IActionResult> PostRetaurant([FromBody] AddRestaurantDTO newRestaurant)
        {
            if (!Enum.TryParse<Category>(newRestaurant.Category, out var restaurantCategory))
            {
                return BadRequest("Invalid category");
            }

            var restaurant = new Restaurant
            {
                Name = newRestaurant.Name,
                Category = restaurantCategory,
                Address = newRestaurant.Address,
                City = newRestaurant.City,
                ZipCode = newRestaurant.ZipCode,
                Latitude = newRestaurant.Latitude,
                Longitude = newRestaurant.Longitude,
                Description = newRestaurant.Description,
                OpeningHours = TimeSpan.Parse(newRestaurant.OpeningHours),
                ClosingHours = TimeSpan.Parse(newRestaurant.ClosingHours),
                ClosingDay = newRestaurant.ClosingDay,
                Phone = newRestaurant.Phone,
                Email = newRestaurant.Email,
            };
            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();
            var restaurantResponse = await _context.Restaurants
                .Where(r => r.IdRestaurant == restaurant.IdRestaurant)
                .Select(r => new RestaurantDTO
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Address = r.Address,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    IsActive = r.IsActive,
                    Image = r.Image
                })
                .FirstOrDefaultAsync();
            return Ok(restaurantResponse);
        }

        // Method to upload an image to a restaurant
        [HttpPut("{idRestaurant}/uploadImage")]
        public async Task<IActionResult> UploadRestaurantImage(int? idRestaurant, [FromBody] IFormFile file)
        {
            if (idRestaurant == null)
            {
                return BadRequest("Missing parameters");
            }
            var restaurant = await _context.Restaurants.FindAsync(idRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid File");
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot", "images", fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            restaurant.Image = fileName;
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return Ok("Image uploaded");

        }

        // method to update a restaurant
        [HttpPut("{idRestaurant}")]
        public async Task<IActionResult> UpdateRestaurant(int? idRestaurant, [FromBody] UpdateRestaurantDTO updatedRestaurant)
        {
            if (idRestaurant == null)
            {
                return BadRequest("Missing parameters");
            }
            var restaurant = await _context.Restaurants.FindAsync(idRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }
            if (!Enum.TryParse<Category>(updatedRestaurant.Category, out var restaurantCategory))
            {
                return BadRequest("Invalid category");
            }
            restaurant.Name = updatedRestaurant.Name;
            restaurant.Category = restaurantCategory;
            restaurant.Address = updatedRestaurant.Address;
            restaurant.City = updatedRestaurant.City;
            restaurant.ZipCode = updatedRestaurant.ZipCode;
            restaurant.Latitude = updatedRestaurant.Latitude;
            restaurant.Longitude = updatedRestaurant.Longitude;
            restaurant.Description = updatedRestaurant.Description;
            restaurant.OpeningHours = TimeSpan.Parse(updatedRestaurant.OpeningHours);
            restaurant.ClosingHours = TimeSpan.Parse(updatedRestaurant.ClosingHours);
            restaurant.ClosingDay = updatedRestaurant.ClosingDay;
            restaurant.Phone = updatedRestaurant.Phone;
            restaurant.Email = updatedRestaurant.Email;
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            var restaurantResponse = await _context.Restaurants
                .Where(r => r.IdRestaurant == restaurant.IdRestaurant)
                .Select(r => new RestaurantDTO
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Address = r.Address,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    IsActive = r.IsActive,
                    Image = r.Image
                })
                .FirstOrDefaultAsync();
            return Ok(restaurantResponse);
        }
        // Method to activate or deactivate a restaurant
        [HttpDelete("{idRestaurant}")]
        public async Task<IActionResult> ToggleRestaurant(int? idRestaurant)
        {
            if (idRestaurant == null)
            {
                return BadRequest("Missing parameters");
            }
            var restaurant = await _context.Restaurants.FindAsync(idRestaurant);
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }
            restaurant.IsActive = !restaurant.IsActive;
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            var restaurantResponse = await _context.Restaurants
                .Where(r => r.IdRestaurant == restaurant.IdRestaurant)
                .Select(r => new RestaurantDTO
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Address = r.Address,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    IsActive = r.IsActive,
                    Image = r.Image
                })
                .FirstOrDefaultAsync();
            return Ok(restaurantResponse);
        }

    }
}

