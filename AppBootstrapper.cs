using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShiftCheck.Services;
using ShiftCheck.ViewModels;

namespace ShiftCheck
{
    public static class AppBootstrapper
    {
        private static readonly IServiceProvider Services = CreateServices();

        public static T Get<T>() { return Services.GetRequiredService<T>(); }

        private static IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddDebug());
            services.AddHttpClient("QuimiOSHub", client =>
            {
                client.BaseAddress = new Uri(HubConnectionSettings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddTransient<LoginViewModel>();
            services.AddSingleton<PendingSamplesViewModel>();
            services.AddTransient<CreateHandoverViewModel>();
            return services.BuildServiceProvider();
        }
    }
}
