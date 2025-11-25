using Microsoft.AspNetCore.Mvc;
using LibraryWeb.Models;
using LibraryWeb.Services;
namespace LibraryWeb.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IBookService _bookService;
        private readonly IBorrowingService _borrowingService;

        public BorrowingController(IApiService apiService, IBorrowingService borrowingService, IBookService bookService )
        {
            _apiService = apiService;
            _borrowingService = borrowingService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để mượn sách!";
                return RedirectToAction("Login", "Auth");
            }

            var book = await _bookService.GetBookDetailAsync(bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sách!";
                return RedirectToAction("Index", "Books");
            }

            if (book.AvailableQuantity <= 0)
            {
                TempData["ErrorMessage"] = "Sách đã hết!";
                return RedirectToAction("Index", "Books");
            }

            var model = new CreateBorrowingViewModel
            {
                BookId = bookId,
                BookTitle = book.Title,
                BookIsbn = book.Isbn,
                BookPrice = book.Price
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBorrowingViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn!";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                var book = await _bookService.GetBookDetailAsync(model.BookId);
                if (book is not null)
                {
                    model.BookTitle = book.Title;
                    model.BookIsbn = book.Isbn;
                    model.BookPrice = book.Price;
                }
                return View(model);
            }

            var result = await _borrowingService.CreateBorrowingAsync(model, userId.Value);
            if (result == null)
            {
                TempData["ErrorMessage"] = "Không thể tạo phiếu mượn!";
                return View(model);
            }

            TempData["SuccessMessage"] = $"Mượn sách thành công! Phiếu #{result.TicketId}. Hạn trả: {result.DueDate:dd/MM/yyyy}";
            return RedirectToAction("MyBorrowings");
        }

        [HttpGet]
        public async Task<IActionResult> MyBorrowings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Auth");
            }

            var borrowings = await _borrowingService.GetMyBorrowingsAsync(userId.Value);
            ViewBag.UserName = HttpContext.Session.GetString("FullName");
            return View(borrowings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int bookId, int ticketId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn!";
                return RedirectToAction("Login", "Auth");
            }

            var success = await _borrowingService.ReturnBookAsync(ticketId, bookId);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Trả sách thành công!" : "Không thể trả sách!";

            return RedirectToAction("MyBorrowings");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Auth");
            }

            var borrowings = await _borrowingService.GetMyBorrowingsAsync(userId.Value);
            var ticket = borrowings.FirstOrDefault(b => b.TicketId == id);

            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu mượn!";
                return RedirectToAction("MyBorrowings");
            }

            return View(ticket);
        }

    }
}
