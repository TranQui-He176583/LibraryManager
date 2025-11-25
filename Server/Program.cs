using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Server.Controllers;
using Server.Models;
using Server.Services;
namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DbContext
            var connectionString = builder.Configuration.GetConnectionString("MyCnn");
            builder.Services.AddDbContext<LibraryManagementDbContext>(options =>
                options.UseSqlServer(connectionString));

           //Register Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IBorrowingService, BorrowingService>();
            var app = builder.Build();


            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "images")),
                RequestPath = "/images"
            });
            
            app.Run("http://localhost:5000");
        }
    }
}
