using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;

namespace Server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly LibraryManagementDbContext _context;

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
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<CategoryDTO>>> CreateCategory(CategoryDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CategoryName))
                    return new ApiResponseDTO<CategoryDTO> { Success = false, Message = "Tên thể loại không được để trống" };

                var category = new Category { CategoryName = request.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<CategoryDTO>
                {
                    Success = true,
                    Message = "Thêm thể loại thành công",
                    Data = new CategoryDTO { CategoryId = category.CategoryId, CategoryName = category.CategoryName }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<CategoryDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<CategoryDTO>>> UpdateCategory(int id, CategoryDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CategoryName))
                    return new ApiResponseDTO<CategoryDTO> { Success = false, Message = "Tên thể loại không được để trống" };

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return new ApiResponseDTO<CategoryDTO> { Success = false, Message = "Không tìm thấy thể loại" };

                category.CategoryName = request.CategoryName;
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<CategoryDTO>
                {
                    Success = true,
                    Message = "Cập nhật thể loại thành công",
                    Data = new CategoryDTO { CategoryId = category.CategoryId, CategoryName = category.CategoryName }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<CategoryDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<bool>>> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Books)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không tìm thấy thể loại" };

                if (category.Books != null && category.Books.Any())
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không thể xóa thể loại này vì đã có sách thuộc thể loại này." };

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<bool>
                {
                    Success = true,
                    Message = "Xóa thể loại thành công",
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
