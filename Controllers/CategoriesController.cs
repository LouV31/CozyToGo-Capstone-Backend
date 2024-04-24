using CozyToGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CozyToGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => c.ToString()).ToList();
            return Ok(categories);
        }
    }
}
