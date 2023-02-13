using System.IO.Compression;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

#region Services
#region Cookie 认证服务
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(opts =>
{
    opts.Cookie.Name = "store_vue_identity";
});
#endregion
#region Session

builder.Services.AddSession(opts =>
{
    // Set a short timeout for easy testing.
    opts.IdleTimeout = TimeSpan.FromSeconds(10);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
    // Make the session cookie essential
    opts.Cookie.IsEssential = true;
});
#endregion
#region GZIP 支持

builder.Services.Configure<GzipCompressionProviderOptions>(opts =>
{
    opts.Level = CompressionLevel.Optimal;
});
builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true;
    opts.Providers.Add<GzipCompressionProvider>();
});
#endregion
#endregion

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();