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

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<AuthorDTO>>> Create([FromBody] AuthorDTO request)
        {
            return await _authorService.CreateAuthor(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO<AuthorDTO>>> Update(int id, [FromBody] AuthorDTO request)
        {
            return await _authorService.UpdateAuthor(id, request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO<bool>>> Delete(int id)
        {
            return await _authorService.DeleteAuthor(id);
        }
    }
}
