using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NetCoreSsrTest.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;

namespace NetCoreSsrTest.Tests.Integration;

public class MoviesEndpointsTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _c;

    public MoviesEndpointsTests(TestWebAppFactory f)
    {
        _c = f.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Admin_Can_Create_Movie()
    {
        var token = await AuthHelper.Login(_c, "admin@local", "Admin123!");
        _c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _c.PostAsJsonAsync("/movies", new { title = "Test", description = "d", year = 2024 });
        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Regular_Cannot_Create_Movie()
    {
        var token = await AuthHelper.Login(_c, "regular@local", "Regular123!");
        _c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _c.PostAsJsonAsync("/movies", new { title = "X", description = "d", year = 2024 });
        res.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Regular_Can_Get_Detail()
    {
        var admin = await AuthHelper.Login(_c, "admin@local", "Admin123!");
        _c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admin);

        var created = await _c.PostAsJsonAsync("/movies", new { title = "M1", description = "d", year = 1977 });
        var body = await created.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = body!["id"].ToString();

        var regular = await AuthHelper.Login(_c, "regular@local", "Regular123!");
        _c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", regular);

        var res = await _c.GetAsync($"/movies/{id}");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
