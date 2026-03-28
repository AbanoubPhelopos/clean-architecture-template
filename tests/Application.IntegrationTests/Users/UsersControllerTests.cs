using System.Net.Http.Json;
using Application.Abstractions.Data;
using Domain.Authorization;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Application.IntegrationTests.Users;

public class UsersControllerTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsersControllerTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.EnsureCreated();
                });
            });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnOkWithUserId()
    {
        var request = new
        {
            email = "newuser@example.com",
            firstName = "New",
            lastName = "User",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/users/register", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var userId = await response.Content.ReadFromJsonAsync<Guid>();
        userId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        var request = new
        {
            email = "invalid-email",
            firstName = "New",
            lastName = "User",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/users/register", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldReturnNotFound()
    {
        var request = new
        {
            email = "nonexistent@example.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/users/login", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}