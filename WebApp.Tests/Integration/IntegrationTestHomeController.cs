using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebApp.Tests.Integration;

[Collection("Database tests")]
public class IntegrationTestHomeController : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;


    public IntegrationTestHomeController(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        CustomWebApplicationFactory<Program>.UseTestAuth = false;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }


    [Fact]
    public async Task Get_Index_IsSuccessful()
    {
        // Arrange
             
        // Act
        var response = await _client.GetAsync("/");
        
        // Assert
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Launch Your Meal Delivery Company", html);
        Assert.Contains("Log in", html);
        Assert.Contains("Register new company", html);
    }

    [Fact]
    public async Task Get_SlugScopedOperationalRoute_Anonymous_IsChallenged()
    {
        // Act
        var response = await _client.GetAsync("/north-bites/CompanySettings");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_UnknownSlugOperationalRoute_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/unknown-company/CompanySettings");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_DeliveryLogistics_Anonymous_IsRedirectedToLogin()
    {
        // Act
        var response = await _client.GetAsync("/north-bites/delivery-logistics");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.OriginalString);
        Assert.Contains("delivery-logistics", response.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task Get_DeliveryLogistics_WithDateFilter_Anonymous_RetainsReturnUrl()
    {
        // Act
        var response = await _client.GetAsync("/north-bites/delivery-logistics?deliveryDate=2026-03-09");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.OriginalString);
        Assert.Contains("deliveryDate%3D2026-03-09", response.Headers.Location.OriginalString);
    }

}
