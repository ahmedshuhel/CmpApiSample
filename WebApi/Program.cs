using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var configuration = app.Configuration.GetSection("Optimizely");
string? clientId = configuration.GetValue<string>("ClientId");
string? clientSecret = configuration.GetValue<string>("ClientSecret");
string? redUri = configuration.GetValue<string>("RedirectUri");

var authUrl = $"https://accounts.cmp.optimizely.com/o/oauth2/v1/auth?client_id={clientId}&redirect_uri={redUri}&response_type=code&scope=openid%20profile%20offline_access";

Console.WriteLine();
Console.WriteLine(authUrl);

app.MapGet("/auth", (ctx) =>
{
    ctx.Response.Redirect(authUrl);
    return Task.CompletedTask;
});

app.MapGet("/auth/callback", async (IMemoryCache cache, HttpContext ctx) =>
{
    // Construct a web request to send to the authorization server to get the access token
    var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.cmp.optimizely.com/o/oauth2/v1/token")
    {
        Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = ctx.Request.Query["code"],
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redUri
        })
    };

    using var client = new HttpClient();
    var response = await client.SendAsync(request);

    // Read the response body and deserialize the JSON
    var responseString = await response.Content.ReadAsStringAsync();
    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseString);

    // Store the tokens in some sort of persistent storage. For this example we'll use the in-memory cache
    cache.Set("token", tokenResponse);

    await ctx.Response.WriteAsync("Success! You can now close this window and can make requests to the API using the token stored in the cache at /assets");
});

app.MapGet("/assets", async (IMemoryCache cache) =>
{
    var token = cache.Get<TokenResponse>("token");

    using var client = new HttpClient();
    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://api.cmp.optimizely.com/v3/assets")
    {
        Headers =
        {
            Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken)
        }
    });

    var responseString = await response.Content.ReadAsStringAsync();
    var root = JsonSerializer.Deserialize<AssetsResponse>(responseString);

    return Results.Ok(root);
});

app.Run();
