using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.Helpers
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;

        // Use IServiceProvider to avoid circular dependency if IAuthService is injected directly
        // because IAuthService also uses HttpClient.
        public AuthHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authService = _serviceProvider.GetService<IAuthService>();
            
            if (authService != null && authService.IsAuthenticated && !string.IsNullOrEmpty(authService.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
