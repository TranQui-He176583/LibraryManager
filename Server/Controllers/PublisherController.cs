using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetPublishers()
        {
            return await _publisherService.GetAllPublishers();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id)
        {
            return await _publisherService.GetPublisher(id);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> Create([FromBody] PublisherDTO request)
        {
            return await _publisherService.CreatePublisher(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> Update(int id, [FromBody] PublisherDTO request)
        {
            return await _publisherService.UpdatePublisher(id, request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO<bool>>> Delete(int id)
        {
            return await _publisherService.DeletePublisher(id);
        }
    }
}
