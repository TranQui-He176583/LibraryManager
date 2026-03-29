using LibraryWeb.Services;
using LibraryWeb.Helpers;
var builder = WebApplication.CreateBuilder(args);
//
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthHandler>();

builder.Services.AddHttpClient<IApiService, ApiService>()
    .AddHttpMessageHandler<AuthHandler>();
builder.Services.AddHttpClient<IBookService, BookService>()
    .AddHttpMessageHandler<AuthHandler>();
builder.Services.AddHttpClient<IBorrowingService, BorrowingService>()
    .AddHttpMessageHandler<AuthHandler>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Initialize ImageUrlHelper với configuration
ImageUrlHelper.Initialize(app.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}

app.UseSession();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run("http://localhost:5001");
