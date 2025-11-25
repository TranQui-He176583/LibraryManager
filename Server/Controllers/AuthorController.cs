using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Models;
using Server.Services;
namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<AuthorDTO>>>> GetAll()
        {
            return await _authorService.getAllAuthor();
        }
    }
}
