using LibraryWeb.Models;
using LibraryWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchFilterViewModel filters)
        {
            filters.Authors = await _bookService.GetAuthorsForDropdownAsync();
            filters.Categories = await _bookService.GetCategoriesForDropdownAsync();
            filters.Languages = await _bookService.GetLanguagesForDropdownAsync();
            var result = await _bookService.SearchBooksAsync(filters);
            result.Filters = filters;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookDetailAsync(id);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sách!";
                return RedirectToAction("Index");
            }
            return View(book);
        }
    }
}
