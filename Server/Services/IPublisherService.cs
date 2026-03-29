using Microsoft.AspNetCore.Mvc;
using Server.DTOs;

namespace Server.Services
{
    public interface IPublisherService
    {
        Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetAllPublishers();
        Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id);
        Task<ActionResult<ApiResponseDTO<PublisherDTO>>> CreatePublisher(PublisherDTO request);
        Task<ActionResult<ApiResponseDTO<PublisherDTO>>> UpdatePublisher(int id, PublisherDTO request);
        Task<ActionResult<ApiResponseDTO<bool>>> DeletePublisher(int id);
    }
}
