using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;

namespace Server.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly LibraryManagementDbContext _context;

        public PublisherService(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetAllPublishers()
        {
            try
            {
                var publishers = await _context.Publishers
                    .OrderBy(p => p.PublisherName)
                    .Select(p => new PublisherDTO
                    {
                        PublisherId = p.PublisherId,
                        PublisherName = p.PublisherName,
                        Web = p.Website
                    })
                    .ToListAsync();

                return new ApiResponseDTO<List<PublisherDTO>>
                {
                    Success = true,
                    Message = "Lấy danh sách nhà xuất bản thành công",
                    Data = publishers
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<List<PublisherDTO>>
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách nhà xuất bản: " + ex.Message
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id)
        {
            try
            {
                var p = await _context.Publishers.FindAsync(id);
                if (p == null)
                    return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Không tìm thấy nhà xuất bản" };

                var dto = new PublisherDTO
                {
                    PublisherId = p.PublisherId,
                    PublisherName = p.PublisherName,
                    Web = p.Website
                };

                return new ApiResponseDTO<PublisherDTO>
                {
                    Success = true,
                    Message = "Lấy thông tin nhà xuất bản thành công",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> CreatePublisher(PublisherDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PublisherName))
                    return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Tên nhà xuất bản không được để trống" };

                var p = new Publisher
                {
                    PublisherName = request.PublisherName,
                    Website = request.Web
                };
                _context.Publishers.Add(p);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<PublisherDTO>
                {
                    Success = true,
                    Message = "Thêm nhà xuất bản thành công",
                    Data = new PublisherDTO { PublisherId = p.PublisherId, PublisherName = p.PublisherName, Web = p.Website }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> UpdatePublisher(int id, PublisherDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PublisherName))
                    return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Tên nhà xuất bản không được để trống" };

                var p = await _context.Publishers.FindAsync(id);
                if (p == null)
                    return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Không tìm thấy nhà xuất bản" };

                p.PublisherName = request.PublisherName;
                p.Website = request.Web;
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<PublisherDTO>
                {
                    Success = true,
                    Message = "Cập nhật nhà xuất bản thành công",
                    Data = new PublisherDTO { PublisherId = p.PublisherId, PublisherName = p.PublisherName, Web = p.Website }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PublisherDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<bool>>> DeletePublisher(int id)
        {
            try
            {
                var p = await _context.Publishers
                    .Include(x => x.Books)
                    .FirstOrDefaultAsync(x => x.PublisherId == id);

                if (p == null)
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không tìm thấy nhà xuất bản" };

                if (p.Books != null && p.Books.Any())
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không thể xóa nhà xuất bản này vì đã có sách thuộc nhà xuất bản này." };

                _context.Publishers.Remove(p);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<bool>
                {
                    Success = true,
                    Message = "Xóa nhà xuất bản thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<bool> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }
    }
}
