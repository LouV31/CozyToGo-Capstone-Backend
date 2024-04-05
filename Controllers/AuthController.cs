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
                City = newUser.City,
                Address = newUser.Address,
                ZipCode = newUser.ZipCode,
                Phone = newUser.Phone,
                Role = "User"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(newUser);

        }

        [HttpPost("authentication")]
        public async Task<IActionResult> LogIn(LoginDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid User");
            }
            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password)) // Qui mancava una parentesi
            {
                return BadRequest("Invalid Password");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name, user.Email.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString())
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var authenticatedUser = new
            {
                Id = user.IdUser,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                City = user.City,
                Address = user.Address,
                Phone = user.Phone,
                Token = tokenHandler.WriteToken(token)
            };

            // Convert authenticatedUser to a dictionary so we can add properties conditionally
            var result = new RouteValueDictionary(authenticatedUser);

            if (!string.IsNullOrEmpty(user.Address2))
            {
                result.Add("Address2", user.Address2);
            }

            if (!string.IsNullOrEmpty(user.Address3))
            {
                result.Add("Address3", user.Address3);
            }

            return Ok(result);
        }
    }
}
