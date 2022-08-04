using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealWebAppAPI;
using RealWorldApp.Commons.Models.UserModel;
using RealWorldApp.DAL;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace RealWorldApp.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        protected readonly HttpClient _httpClient;

        protected IntegrationTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        if (dbContext != null)
                        {
                            services.Remove(dbContext);
                        }
                        var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryEmployeeTest");
                            options.UseInternalServiceProvider(serviceProvider);
                        });
                        var sp = services.BuildServiceProvider();

                        using (var scope = sp.CreateScope())
                        {
                            using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                try
                                {
                                    appContext.Database.EnsureCreated();
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                            }
                        }
                    });
                });
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        protected async Task AuthenticateAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            Regex rx = new Regex(@"[a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string registerResponse;

            var checkUser = await _httpClient.PostAsJsonAsync("api/users/login", new UserLoginContainer 
            { 
                User = new UserLogin 
                { 
                    Email = "integration@test.com",
                    Password = "zaq1@WSX"   
                }
            });

            if (!checkUser.IsSuccessStatusCode)
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", new UserRegisterContainer
                {
                    User = new UserRegister
                    {
                        Email = "integration@test.com",
                        Username = "integration",
                        Password = "zaq1@WSX"
                    }
                });

                registerResponse = await response.Content.ReadAsStringAsync();

                MatchCollection matches = rx.Matches(registerResponse);

                return matches[0].ToString();
            }
            else
            {
                registerResponse = await checkUser.Content.ReadAsStringAsync();

                MatchCollection matches = rx.Matches(registerResponse);

                return matches[0].ToString();
            }
        }
    }
}
