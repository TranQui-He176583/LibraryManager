using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
namespace Server.Services
{
    public class CategoryService : ICategoryService
    {
        public LibraryManagementDbContext _context;
        public CategoryService(LibraryManagementDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<ApiResponseDTO<List<CategoryDTO>>>> GetAllCategory()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .Select(c => new CategoryDTO
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    })
                    .ToListAsync();

                return new ApiResponseDTO<List<CategoryDTO>>
                {
                    Success = true,
                    Message = "Lấy danh sách thể loại thành công!",
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<List<CategoryDTO>>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra!" + ex.Message

                };
            }
        }
    }
    
}
