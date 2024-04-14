using CozyToGo.Data;
using CozyToGo.DTO.AddressDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AddressesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Get all addresses of the Authenticated User
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var addresses = await _context.Addresses
                .Where(a => a.IdUser.ToString() == userId)
                .Select(a => new AddressDTO
                {
                    IdAddress = a.IdAddress,
                    IdUser = a.IdUser,
                    AddressName = a.AddressName,
                    StreetAddress = a.StreetAddress,
                    City = a.City,
                    ZipCode = a.ZipCode,
                    CompleteAddress = a.CompleteAddress,
                    IsPrimary = a.IsPrimary
                })
                .ToListAsync();
            return Ok(addresses);
        }
        // Create a new address for the Authenticated User
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAddress([FromBody] AddAddressDTO addressDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var address = new Address
            {
                IdUser = Convert.ToInt32(userId),
                AddressName = addressDTO.AddressName,
                StreetAddress = addressDTO.StreetAddress,
                City = addressDTO.City,
                ZipCode = addressDTO.ZipCode,
                IsPrimary = false
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            var addressResponse = new AddressDTO
            {
                IdAddress = address.IdAddress,
                IdUser = address.IdUser,
                AddressName = address.AddressName,
                CompleteAddress = address.CompleteAddress
            };
            return Ok(addressResponse);
        }
        // Edit an address of the Authenticated User
        [Authorize]
        [HttpPut("{idAddress}")]
        public async Task<IActionResult> PutAddress(int idAddress, [FromBody] EditAddressDTO addressDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (idAddress != addressDTO.IdAddress)
            {
                return BadRequest();
            }

            var address = await _context.Addresses.FindAsync(idAddress);
            if (address == null)
            {
                return NotFound();
            }


            address.AddressName = addressDTO.AddressName;
            address.StreetAddress = addressDTO.StreetAddress;
            address.City = addressDTO.City;
            address.ZipCode = addressDTO.ZipCode;


            _context.Update(address);
            await _context.SaveChangesAsync();
            return Ok(address);
        }

        // Set an address as primary for the Authenticated User
        [Authorize]
        [HttpPut("SetPrimary/{idAddress}")]
        public async Task<IActionResult> SetPrimary(int? idAddress)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (idAddress == null)
            {
                return BadRequest("Missing Parameters");
            }
            var address = await _context.Addresses.FindAsync(idAddress);
            if (address == null)
            {
                return NotFound();
            }

            var addresses = await _context.Addresses
                .Where(a => a.IdUser.ToString() == userId)
                .ToListAsync();
            foreach (var a in addresses)
            {
                a.IsPrimary = false;
            }
            address.IsPrimary = true;
            await _context.SaveChangesAsync();
            return Ok(address);
        }


        [HttpDelete("AddressId/{idAddress}")]
        public async Task<IActionResult> DeleteAddress(int idAddress, int idUser)
        {
            var address = await _context.Addresses.FindAsync(idAddress);
            if (address == null)
            {
                return NotFound();
            }

            if (address.IdUser != idUser)
            {
                return Unauthorized("You can not delete this address");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok("Succesfully deleted");
        }
    }
}
