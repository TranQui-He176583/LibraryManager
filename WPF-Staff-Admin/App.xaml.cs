using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System;
using System.IO;
using System.Windows;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.ViewModels;
using WPF_Staff_Admin.ViewModels.Borrowing;
using WPF_Staff_Admin.Views;
using System.Runtime.InteropServices;
using System.Net.Http;
namespace WPF_Staff_Admin
{
    public partial class App : Application
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool AllocConsole();
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            //AllocConsole();
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddTransient<WPF_Staff_Admin.Helpers.AuthHandler>();

            services.AddHttpClient("ApiClient")
                .AddHttpMessageHandler<WPF_Staff_Admin.Helpers.AuthHandler>();

            services.AddHttpClient("AuthClient");

            // Services
            services.AddSingleton<IApiService>(sp => 
                new ApiService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"),
                    sp.GetRequiredService<IConfiguration>()
                ));

            services.AddSingleton<IFileUploadService>(sp => 
                new FileUploadService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"),
                    sp.GetRequiredService<IConfiguration>()
                ));

            services.AddSingleton<IBorrowingService>(sp => 
                new BorrowingService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"),
                    sp.GetRequiredService<IConfiguration>()
                ));

            services.AddSingleton<IAuthService>(sp => 
                new AuthService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"),
                    sp.GetRequiredService<IConfiguration>()
                ));

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IBookService, BookService>();           
            services.AddSingleton<IAuthorService, AuthorService>();       
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IPublisherService, PublisherService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<BookListViewModel>();
            services.AddTransient<BorrowingListViewModel>();


            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }
       
    }
    public static class ServiceProviderExtensions
    {
        public static T? GetService<T>(this IServiceProvider provider)
        {
            return (T?)provider.GetService(typeof(T));
        }
    }

}
