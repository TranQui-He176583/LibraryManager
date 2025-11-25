using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Services;
namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private IBookService _bookService;

        public PublisherController(IBookService bookService)
        {
           _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetPublishers()
        {
           return await _bookService.GetPublishers();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id)
        {
           return await _bookService.GetPublisher(id);
        }
    }
}
