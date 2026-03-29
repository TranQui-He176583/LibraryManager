using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryWeb.Models;

using LibraryWeb.Services;

namespace LibraryWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookService _bookService;

    public HomeController(ILogger<HomeController> logger, IBookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        // Try to fetch the latest 8 books. Assuming PageSize is enough, default sorting usually brings up recent additions.
        var searchResult = await _bookService.SearchBooksAsync(new SearchFilterViewModel 
        { 
            PageSize = 8, 
            SortBy = "BookId", // Using BookId to roughly get the newest ones if auto-increment
            SortOrder = "desc", 
            Page = 1 
        });

        var model = new HomeViewModel
        {
            RecentBooks = searchResult?.Books ?? new List<BookDetailViewModel>()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
