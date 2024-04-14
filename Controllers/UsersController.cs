using CozyToGo.Data;
using CozyToGo.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("Missing Parameters");
            }
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Where(u => u.IdUser.ToString() == userId)
                .Select(u => new UserDTO
                {
                    IdUser = u.IdUser,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Addresses
                        .Where(a => a.IsPrimary)
                        .Select(a => a.CompleteAddress)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
            return Ok(user);
        }
        [Authorize]
        [HttpPut("editUser")]
        public async Task<IActionResult> EditUser([FromBody] EditUserDTO userDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }


            var user = await _context.Users.Where(u => u.IdUser.ToString() == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Email = userDTO.Email;
            user.Phone = userDTO.Phone;

            _context.Update(user);
            await _context.SaveChangesAsync();


            return Ok(new { user.Email, user.Phone });
        }



        [HttpPut("changePassword/{idUser}")]
        public async Task<IActionResult> ChangePassword(int idUser, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (idUser != changePasswordDTO.IdUser)
            {
                return BadRequest("User ID mismatch");
            }

            var user = await _context.Users.FindAsync(idUser);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verifica che la vecchia password sia corretta
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, user.Password))
            {
                return BadRequest("Old password is incorrect");
            }

            // Verifica che la nuova password e la conferma della nuova password coincidano
            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
            {
                return BadRequest("New password and confirm new password do not match");
            }

            // Cripta la nuova password e salvala nel database
            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}