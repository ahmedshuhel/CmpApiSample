using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OptimizelySettings>(builder.Configuration.GetSection("Optimizely"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapGet("/auth/callback", async (IMemoryCache cache, IOptions<OptimizelySettings> settings, HttpContext ctx) =>
{
    // Construct a web request to send to the authorization server to get the access token
    var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.cmp.optimizely.com/o/oauth2/v1/token")
    {
        Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = settings.Value.ClientId,
            ["client_secret"] = settings.Value.ClientSecret,
            ["code"] = ctx.Request.Query["code"],
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = settings.Value.RedirectUri
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
})
.WithDescription("Callback for the OAuth flow")
.ExcludeFromDescription();

app.MapGet("/auth", (IOptions<OptimizelySettings> settings) =>
{
    var authUrl = string.Format(
        settings.Value.AuthUrlTemplate,
        settings.Value.ClientId,
        settings.Value.RedirectUri
    );
    return Results.Redirect(authUrl);
})
.WithDescription("Begin the OAuth flow")
.Produces(StatusCodes.Status307TemporaryRedirect);

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
})
.WithDescription("Get a list of assets")
.Produces<AssetsResponse>(StatusCodes.Status200OK);

app.Run();
