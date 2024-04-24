using CozyToGo.Data;
using CozyToGo.DTO.UserDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO newUser)
        {
            if (newUser == null)
            {
                return BadRequest("User data is required");
            }
            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
            {
                return BadRequest("Email already exist");
            }
            var user = new User
            {
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Phone = newUser.Phone,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var address = new Address
            {
                IdUser = user.IdUser,
                AddressName = "Casa",
                StreetAddress = newUser.StreetAddress,
                City = newUser.City,
                ZipCode = newUser.ZipCode,
                IsPrimary = true
            };
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return Ok(newUser);

        }


        [HttpPost("authentication")]
        public async Task<IActionResult> LogIn(LoginDTO userDto)
        {
            var user = await _context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Email == userDto.Email);
            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Email == userDto.Email);
            if (user == null && owner == null)
            {
                return BadRequest("Invalid User");
            }


            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password)) // Qui mancava una parentesi
                {
                    return BadRequest("Invalid Password");
                }
                // Generate JWT Token for the user
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
    new Claim(ClaimTypes.Name, user.Email.ToString()),
    new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
    new Claim(ClaimTypes.Role, user.Role.ToString())
}),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Ottieni l'indirizzo principale dell'utente
                var mainAddress = user.Addresses.FirstOrDefault(a => a.IsPrimary);

                var authenticatedUser = new
                {
                    Id = user.IdUser,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = mainAddress != null ? mainAddress.CompleteAddress : null,
                    Token = tokenHandler.WriteToken(token),
                    Role = user.Role
                };



                return Ok(authenticatedUser);
            }
            else
            {
                if (!BCrypt.Net.BCrypt.Verify(userDto.Password, owner.Password))
                {
                    return BadRequest("Invalid Password");
                }
                // Generate JWT Token for the owner
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
    new Claim(ClaimTypes.Name, owner.Email.ToString()),
    new Claim(ClaimTypes.NameIdentifier, owner.IdOwner.ToString()),
    new Claim(ClaimTypes.Role, owner.Role.ToString())
}),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);



                var authenticatedOwner = new
                {
                    Id = owner.IdOwner,
                    Name = owner.Name,
                    Surname = owner.Surname,
                    Email = owner.Email,
                    Role = owner.Role,
                    Token = tokenHandler.WriteToken(token),
                };
                return Ok(authenticatedOwner);

            }


        }
    }
}
