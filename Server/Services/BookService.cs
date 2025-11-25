using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Server.DTOs;
using Server.Models;
using System.Threading.Tasks;

namespace Server.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryManagementDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public BookService(LibraryManagementDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _environment = webHostEnvironment;
        }

        public async Task<List<BookDetailDTO>> GetAllBook()
        {
            var booksId = _context.Books.Select(x => x.BookId).ToList();
            List<BookDetailDTO> books = new List<BookDetailDTO>();
            foreach (int i in booksId)
            {
                var bookDetail = await ReturnBookDetail(i);
                books.Add(bookDetail);
            }
            return books.OrderBy(b => b.Title).ToList();
        }


        public async Task<BookDetailDTO> ReturnBookDetail(int bookId)
        {

            var book = await _context.Books
                .Include(b => b.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Where(bd => bd.BookId == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
                throw new Exception($"Thư viện không có sách tương ứng với Id này");


            return new BookDetailDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Isbn = book.Isbn,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.PublisherName,
                PublicationYear = book.PublishedYear,
                TotalQuantity = book.TotalQuantity,
                AvailableQuantity = book.AvailableQuantity,
                Price = book.Price,
                ImageUrl = book.ImageUrl,
                Language = book.Language,
                Location = book.Location,
                Authors = MapToAuthorDTO(book),
                Description = book.Description,
                PageCount = book.PageCount,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                Publisher = book.Publisher?.PublisherName,
                Categories = MapToCategoryDTO(book),
            };
        }

        public List<AuthorDTO> MapToAuthorDTO(Book book)
        {
            var authorDTOs = new List<AuthorDTO>();
            foreach (Author a in book.Authors.ToList())
            {
                authorDTOs.Add(new AuthorDTO { AuthorId = a.AuthorId, AuthorName = a.AuthorName });
            }
            return authorDTOs;

        }

        public List<CategoryDTO> MapToCategoryDTO(Book book)
        {
            var categoryDTOs = new List<CategoryDTO>();
            foreach (Category c in book.Categories.ToList())
            {
                categoryDTOs.Add(new CategoryDTO { CategoryId = c.CategoryId, CategoryName = c.CategoryName });
            }
            return categoryDTOs;

        }

        public async Task<ActionResult<ApiResponseDTO<SearchBooksResultDTO>>> SearchBooks([FromQuery] SearchBooksRequestDTO request)
        {
            try
            {
                var query = _context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.Categories)
                    .Include(b => b.Authors)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.SearchQuery))
                {
                    var searchLower = request.SearchQuery.ToLower();
                    query = query.Where(b => b.Title.ToLower().Contains(searchLower));
                }

                if (request.AuthorId.HasValue)
                {
                    query = query.Where(b => b.Authors.Any(ba => ba.AuthorId == request.AuthorId.Value));
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(b => b.Categories.Any(bc => bc.CategoryId == request.CategoryId.Value));
                }

                if (!string.IsNullOrEmpty(request.Language))
                {
                    query = query.Where(b => b.Language == request.Language);
                }

                query = request.SortBy.ToLower() switch
                {
                    "year" => request.SortOrder == "desc"
                        ? query.OrderByDescending(b => b.PublishedYear)
                        : query.OrderBy(b => b.PublishedYear),
                    _ => request.SortOrder == "desc"
                        ? query.OrderByDescending(b => b.Title)
                        : query.OrderBy(b => b.Title)
                };

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

                var books = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var bookDtos = books.Select(b => new BookDetailDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Isbn = b.Isbn,
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher?.PublisherName,
                    PublicationYear = b.PublishedYear,
                    TotalQuantity = b.TotalQuantity,
                    Authors = MapToAuthorDTO(b),
                    AvailableQuantity = b.AvailableQuantity,
                    Categories = MapToCategoryDTO(b),
                    Price = b.Price,
                    ImageUrl = b.ImageUrl,
                    Language = b.Language,
                    Location = b.Location
                }).ToList();

                var result = new SearchBooksResultDTO
                {
                    Books = bookDtos,
                    CurrentPage = request.Page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    PageSize = request.PageSize
                };

                return new ApiResponseDTO<SearchBooksResultDTO>
                {
                    Success = true,
                    Message = "Tìm kiếm thành công!",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<SearchBooksResultDTO>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm!" + ex.Message,
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<List<string>>>> GetLanguages()
        {
            try
            {
                var languages = await _context.Books
                    .Where(b => !string.IsNullOrEmpty(b.Language))
                    .Select(b => b.Language)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToListAsync();

                return new ApiResponseDTO<List<string>>
                {
                    Success = true,
                    Message = "Lấy danh sách ngôn ngữ thành công!",
                    Data = languages!
                };
            }
            catch (Exception ex)
            {

                return new ApiResponseDTO<List<string>>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra!" + ex.Message
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetPublishers()
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
                    Message = "Lỗi khi lấy danh sách nhà xuất bản" + ex.Message,

                };
            }

        }

        public async Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id)
        {
            try
            {
                var publisher = await _context.Publishers
                    .Where(p => p.PublisherId == id)
                    .Select(p => new PublisherDTO
                    {
                        PublisherId = p.PublisherId,
                        PublisherName = p.PublisherName,
                        Web = p.Website
                    })
                    .FirstOrDefaultAsync();

                if (publisher == null)
                {
                    return new ApiResponseDTO<PublisherDTO>
                    {
                        Success = false,
                        Message = "Không tìm thấy nhà xuất bản"
                    };
                }

                return new ApiResponseDTO<PublisherDTO>
                {
                    Success = true,
                    Message = "Lấy thông tin nhà xuất bản thành công",
                    Data = publisher
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PublisherDTO>
                {
                    Success = false,
                    Message = "Lỗi khi lấy thông tin nhà xuất bản" + ex.Message
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> CreateBook([FromBody] CreateBookRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Tên sách không được để trống"
                    };
                }

                if (request.TotalQuantity < 0 || request.AvailableQuantity < 0)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Số lượng không hợp lệ"
                    };
                }

                if (request.AvailableQuantity > request.TotalQuantity)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Số lượng còn lại không thể lớn hơn tổng số lượng"
                    };
                }

                if (!string.IsNullOrWhiteSpace(request.Isbn))
                {
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.Isbn == request.Isbn);

                    if (existingBook != null)
                    {
                        return new ApiResponseDTO<BookDetailDTO>
                        {
                            Success = false,
                            Message = $"ISBN {request.Isbn} đã tồn tại"
                        };
                    }
                }

                var book = new Book
                {
                    Title = request.Title,
                    Isbn = request.Isbn,
                    PublisherId = request.PublisherId,
                    PublishedYear = request.PublishedYear,
                    PageCount = request.PageCount,
                    Language = request.Language,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    TotalQuantity = request.TotalQuantity,
                    AvailableQuantity = request.AvailableQuantity,
                    Price = request.Price,
                    Location = request.Location,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                if (request.AuthorIds != null && request.AuthorIds.Any())
                {
                    foreach (var authorId in request.AuthorIds)
                    {
                        var author = await _context.Authors
                            .Where(a => a.AuthorId == authorId)
                            .FirstOrDefaultAsync();
                        if (author != null)
                        book.Authors.Add(author);
                    }
                    await _context.SaveChangesAsync();
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    foreach (var categoryId in request.CategoryIds)
                    {
                        var categrory = await _context.Categories
                            .Where(a => a.CategoryId == categoryId)
                            .FirstOrDefaultAsync();
                        if (categrory != null)
                        book.Categories.Add(categrory);
                    }
                    await _context.SaveChangesAsync();
                }

                var createdBook = await _context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .FirstOrDefaultAsync(b => b.BookId == book.BookId);

                var bookDTO = MapToBookDTO(createdBook!);

                return 
                    new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = true,
                        Message = "Thêm sách thành công",
                        Data = bookDTO
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<BookDetailDTO>
                {
                    Success = false,
                    Message = "Lỗi khi thêm sách" + ex.Message,
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> UpdateBook(int id, [FromBody] UpdateBookRequest request)
        {
            try
            {
                if (id != request.BookId)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "ID không khớp"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Tên sách không được để trống"
                    };
                }

                if (request.TotalQuantity < 0 || request.AvailableQuantity < 0)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Số lượng không hợp lệ"
                    };
                }

                if (request.AvailableQuantity > request.TotalQuantity)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Số lượng còn lại không thể lớn hơn tổng số lượng"
                    };
                }

                var book = await _context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    return new ApiResponseDTO<BookDetailDTO>
                    {
                        Success = false,
                        Message = "Không tìm thấy sách"
                    };
                }

                if (!string.IsNullOrWhiteSpace(request.Isbn) && request.Isbn != book.Isbn)
                {
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.Isbn == request.Isbn && b.BookId != id);

                    if (existingBook != null)
                    {
                        return new ApiResponseDTO<BookDetailDTO>
                        {
                            Success = false,
                            Message = $"ISBN {request.Isbn} đã tồn tại"
                        };
                    }
                }

                book.Title = request.Title;
                book.Isbn = request.Isbn;
                book.PublisherId = request.PublisherId;
                book.PublishedYear = request.PublishedYear;
                book.PageCount = request.PageCount;
                book.Language = request.Language;
                book.Description = request.Description;
                book.ImageUrl = request.ImageUrl;
                book.TotalQuantity = request.TotalQuantity;
                book.AvailableQuantity = request.AvailableQuantity;
                book.Price = request.Price;
                book.Location = request.Location;
                book.UpdatedAt = DateTime.Now;

               // _context.BookAuthors.RemoveRange(book.BookAuthors);
                book.Authors.Clear();

                if (request.AuthorIds != null && request.AuthorIds.Any())
                {
                    foreach (var authorId in request.AuthorIds)
                    {
                        var author = await _context.Authors
                            .Where(a => a.AuthorId == authorId)
                            .FirstOrDefaultAsync();
                        if (author != null)
                            book.Authors.Add(author);
                    }
                }
               // _context.BookCategories.RemoveRange(book.BookCategories);
               book.Categories.Clear();
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    foreach (var categoryId in request.CategoryIds)
                    {
                        var categrory = await _context.Categories
                             .Where(a => a.CategoryId == categoryId)
                             .FirstOrDefaultAsync();
                        if (categrory != null)
                            book.Categories.Add(categrory);
                    }
                }

                await _context.SaveChangesAsync();

                var updatedBook = await _context.Books
                    .Include(b => b.Publisher)
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                var bookDTO = MapToBookDTO(updatedBook!);

                return new ApiResponseDTO<BookDetailDTO>
                {
                    Success = true,
                    Message = "Cập nhật sách thành công",
                    Data = bookDTO
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<BookDetailDTO>
                {
                    Success = false,
                    Message = "Lỗi khi cập nhật sách" + ex.Message,
                };
            }
        }

        public async Task<ActionResult<ApiResponseDTO<string>>> UploadBookImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Không có file được chọn"
                    };
                }
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif, .webp)"
                    };
                }
                if (file.Length > 5 * 1024 * 1024)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Kích thước file không được vượt quá 5MB"
                    };
                }

                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "images", "books");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/images/books/{uniqueFileName}";

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Upload ảnh thành công",
                    Data = imageUrl
                };
            }
            catch (Exception ex)
            {
                return  new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Lỗi khi upload ảnh" + ex.Message,
                };
            }
        }

        public ActionResult<ApiResponse> DeleteBookImage([FromQuery] int bookId)
        {
            try
            {
                var imageUrl =  _context.Books
                   .Where(b => b.BookId == bookId)
                   .Select(b => b.ImageUrl)
                   .FirstOrDefault();

                if (imageUrl == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Sách này không có ảnh"
                    };

                }
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.ContentRootPath, "images", "books", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return new ApiResponse
                {
                    Success = true,
                    Message = "Xóa ảnh thành công"
                };
            }
            catch (Exception ex)
            {
                return  new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi khi xóa ảnh" + ex.Message,
                };
            }
        }

        public ActionResult<ApiResponse> DeleteBook([FromQuery] int bookId)
        {
            try
            {
                var book = _context.Books
                    .Where(b => b.BookId == bookId)
                    .Include(b => b.Authors)
                    .Include(b => b.Categories)
                    .Include(b => b.BorrowingDetails)
                    .FirstOrDefault();

                if (book == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không có sách này!"
                    };

                }
                book.Authors.Clear();
                book.Categories.Clear();

                    if (book.BorrowingDetails != null && book.BorrowingDetails.Any())
                    {
                        _context.BorrowingDetails.RemoveRange(book.BorrowingDetails);
                    }
                    _context.Books.Remove(book);
                _context.SaveChanges();
                return new ApiResponse
                {
                    Success = true,
                    Message = "Xóa sách thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi khi xóa sách" + ex.Message,
                };
            }
        }



        private BookDetailDTO MapToBookDTO(Book book)
        {
            return new BookDetailDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Isbn = book.Isbn,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.PublisherName,
                PublicationYear = book.PublishedYear,
                TotalQuantity = book.TotalQuantity,
                AvailableQuantity = book.AvailableQuantity,
                Price = book.Price,
                ImageUrl = book.ImageUrl,
                Language = book.Language,
                Location = book.Location,
                Description = book.Description,
                PageCount = book.PageCount,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                Authors = book.Authors?
                    .Select(ba => new AuthorDTO
                    {
                        AuthorId = ba.AuthorId,
                        AuthorName = ba.AuthorName
                    })
                    .ToList(),
                Categories = book.Categories?
                    .Select(bc => new CategoryDTO
                    {
                        CategoryId = bc.CategoryId,
                        CategoryName = bc.CategoryName
                    })
                    .ToList()
            };

        }
    } 
}