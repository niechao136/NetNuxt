using System.IO.Compression;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using NetNuxt.Models;

var builder = WebApplication.CreateBuilder(args);

#region Services Register
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
#region Controller

builder.Services.AddControllers();
#endregion
#region 跨域服务

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
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
#region SPA 服务

builder.Services.AddSpaStaticFiles(opts =>
{
    opts.RootPath = "wwwroot";
});
#endregion
#region AppSettings 配置

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
#endregion
#endregion

var app = builder.Build();

#region Add Middleware
#region Cookie 认证服务

app.UseAuthentication();
#endregion
#region 跨域服务

app.UseCors();
app.UseCookiePolicy();
#endregion
#region GZIP 支持

app.UseResponseCompression();
#endregion
#region SPA 服务

app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot";
});
#endregion
#region Controller

app.UseRouting();
app.MapControllerRoute("app", "app/{controller}/{action=Index}/{id?}");
#endregion
#endregion

app.Run();