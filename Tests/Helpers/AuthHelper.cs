namespace NetCoreSsrTest.Tests.Helpers;

public static class AuthHelper
{
    public static async Task<string> Login(HttpClient c, string email, string password)
    {
        var res = await c.PostAsJsonAsync("/auth/login", new { email, password });
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return json!["token"];
    }
}
