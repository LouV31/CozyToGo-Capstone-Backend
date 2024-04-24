using CozyToGo.Data;
using CozyToGo.DTO.RestaurantDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        [HttpGet("search/{restaurantName}")]
        public async Task<IActionResult> GetRestaurantByName(string restaurantName)
        {

            var restaurant = await _context.Restaurants
                .Where(r => r.Name.ToLower().Contains(restaurantName.ToLower()))
                .Select(r => new
                {
                    IdRestaurant = r.IdRestaurant,
                    IdOwner = r.IdOwner,
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
            if (restaurant == null)
            {
                return NotFound("Restaurant not found");
            }
            return Ok(restaurant);
        }
        [Authorize(Roles = "Owner")]
        [HttpGet("ownerRestaurant")]
        public async Task<IActionResult> GetRestaurantByOwnerId()
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ownerId == null)
            {
                return Unauthorized("You must be logged in to perform this action");
            }
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Owner")
            {
                return Unauthorized("You must be Owner to perform this action");
            }
            var restaurant = await _context.Restaurants
                .Where(r => r.IdOwner.ToString() == ownerId)
                .Select(r => new
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    Image = r.Image
                }).FirstOrDefaultAsync();
            return Ok(restaurant);

        }

        // Metod to add a new restaurant
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostRetaurant([FromBody] AddRestaurantAndOwnerDTO newRestaurantAndOwner)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("You must be logged in to perform this action");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
            {
                return Unauthorized("You must be Admin to perform this action");
            }
            if (newRestaurantAndOwner == null)
            {
                return BadRequest("Missing parameters");
            }
            if (await _context.Users.AnyAsync(u => u.Email == newRestaurantAndOwner.NewOwner.Email) && await _context.Owners.AnyAsync(o => o.Email == newRestaurantAndOwner.NewOwner.Email))
            {
                return BadRequest("Email already exist");
            }
            var owner = new Owner
            {
                Name = newRestaurantAndOwner.NewOwner.Name,
                Surname = newRestaurantAndOwner.NewOwner.Surname,
                Email = newRestaurantAndOwner.NewOwner.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(newRestaurantAndOwner.NewOwner.Password),
            };
            await _context.Owners.AddAsync(owner);
            await _context.SaveChangesAsync();

            if (!Enum.TryParse<Category>(newRestaurantAndOwner.NewRestaurant.Category, out var restaurantCategory))
            {
                return BadRequest("Invalid category");
            }

            var restaurant = new Restaurant
            {
                IdOwner = owner.IdOwner,
                Name = newRestaurantAndOwner.NewRestaurant.Name,
                Category = restaurantCategory,
                Address = newRestaurantAndOwner.NewRestaurant.Address,
                City = newRestaurantAndOwner.NewRestaurant.City,
                ZipCode = newRestaurantAndOwner.NewRestaurant.ZipCode,
                Latitude = newRestaurantAndOwner.NewRestaurant.Latitude,
                Longitude = newRestaurantAndOwner.NewRestaurant.Longitude,
                Description = newRestaurantAndOwner.NewRestaurant.Description,
                OpeningHours = TimeSpan.Parse(newRestaurantAndOwner.NewRestaurant.OpeningHours),
                ClosingHours = TimeSpan.Parse(newRestaurantAndOwner.NewRestaurant.ClosingHours),
                ClosingDay = newRestaurantAndOwner.NewRestaurant.ClosingDay,
                Phone = newRestaurantAndOwner.NewRestaurant.Phone,
                Email = newRestaurantAndOwner.NewRestaurant.Email,
            };
            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();
            var restaurantResponse = await _context.Restaurants
                .Where(r => r.IdRestaurant == restaurant.IdRestaurant)
                .Select(r => new RestaurantDTO
                {
                    IdRestaurant = r.IdRestaurant,
                    IdOwner = r.IdOwner,
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
        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{idRestaurant}/uploadImage")]
        public async Task<IActionResult> UploadRestaurantImage(int? idRestaurant, IFormFile file)
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

            // Ottieni l'ID dell'utente corrente
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Se l'utente è un Owner, verifica che stia cercando di caricare un'immagine per il proprio ristorante
            if (User.IsInRole("Owner") && restaurant.IdOwner.ToString() != userId)
            {
                return Unauthorized("You can only upload images for your own restaurant");
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
            return Ok(new { image = fileName });
        }

        // method to update a restaurant
        [Authorize(Roles = "Owner")]
        [HttpPatch("{idRestaurant}")]
        public async Task<IActionResult> UpdateRestaurant(int? idRestaurant, UpdateRestaurantDTO updatedRestaurant)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ownerId == null)
            {
                return Unauthorized("You must be logged in to perform this action");
            }
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Owner")
            {
                return Unauthorized("You must be Owner to perform this action");
            }
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
                .Select(r => new
                {
                    IdRestaurant = r.IdRestaurant,
                    Name = r.Name,
                    Category = r.Category.ToString(),
                    Description = r.Description,
                    OpeningHours = r.OpeningHours.ToString(),
                    ClosingHours = r.ClosingHours.ToString(),
                    ClosingDay = r.ClosingDay,
                    Phone = r.Phone,
                    Email = r.Email,
                    Image = r.Image
                })
                .FirstOrDefaultAsync();
            return Ok(restaurantResponse);
        }
        // Method to activate or deactivate a restaurant
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{idRestaurant}")]
        public async Task<IActionResult> ToggleRestaurant(int? idRestaurant)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("You must be logged in to perform this action");
            }
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
            {
                return Unauthorized("You must be Admin to perform this action");
            }
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

