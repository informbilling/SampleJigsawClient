using InformBilling_JigsawClient_Example.APIClient;
using InformBilling_JigsawClient_Example.Models;
using InformBilling_JigsawClient_Example.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class Program
{
    private static IConfiguration _configuration { get; set; }

    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args)
                     .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
                     .Build();

        // Resolves the required classes
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var identitySettings = scope.ServiceProvider.GetService<IOptions<IdentityServerSettingsDto>>();
            var httpBaseSettings = scope.ServiceProvider.GetService<IOptions<HttpClientBaseSettings>>();

            var jigsawClient = scope.ServiceProvider.GetService<IInformHttpClientBase>();
            jigsawClient.InitialiseFromDIResolver(identitySettings, httpBaseSettings.Value);
            var accountService = scope.ServiceProvider.GetService<IAccountService>();
            accountService.GetAccountById();
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                _configuration = hostContext.Configuration;

                var identityServerSettingsSection = _configuration.GetSection("IdentityServerSettings");
                services.Configure<IdentityServerSettingsDto>(identityServerSettingsSection);

                var httpClientBaseSettingsSection = _configuration.GetSection("HttpClientBaseSettings");
                services.Configure<HttpClientBaseSettings>(httpClientBaseSettingsSection);

                services.AddScoped<IInformHttpClientBase, InformHttpClientBase>();
                services.AddScoped<IAccountService, AccountService>();

            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            });

}