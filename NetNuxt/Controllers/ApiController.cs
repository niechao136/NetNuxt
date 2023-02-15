using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetNuxt.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetNuxt.Controllers;

public class ApiController : BaseController
{
    private readonly IOptions<AppSettings> _settings;

    public ApiController(IOptions<AppSettings> settings)
    {
        _settings = settings;
    }

    public string Post([FromBody]JObject data)
    {
        var url = data["url"]?.ToString()!;
        if (!NOT_NEED_TOKEN.Contains(url) && HttpContext.User.Claims.Any())
        {
            return new JObject(new JProperty("status", 2)).ToString();
        }
        var form = data["data"];
        var result = PostRequest(UrlHandler(url), DataHandler(url, form!));
        switch (url)
        {
            case "login":
                Login(result);
                break;
            case "logout":
                Logout(result);
                break;
        }
        return result;
    }
    
    private static readonly string[] POS_URL = { "pos/query", "pos/isposdata", "pos/upload" };
    private string UrlHandler(string url)
    {
        return POS_URL.Contains(url)
            ? _settings.Value.POSAPIPATH?.URL + "/api/" + url
            : _settings.Value.APIPATH?.URL + "/api/" + url;
    }

    private static readonly string[] NOT_NEED_TOKEN = { "" };
    private string DataHandler(string url, JToken data)
    {
        if (NOT_NEED_TOKEN.Contains(url)) return data.ToString();
        var form = new JObject(data);
        var token = JsonConvert.DeserializeObject<JObject>(HttpContext.User.Claims
            .SingleOrDefault(x => x.Type == "token")?.Value!);
        if (POS_URL.Contains(url))
        {
            var pos = new JObject
            {
                new JProperty("token", token)
            };
            form.Add(new JProperty("token", pos));
            return form.ToString();
        }
        form.Add(new JProperty("token", token));
        if (form.ContainsKey("password"))
        {
            var psw = form["password"]?.ToString()!;
            form["password"] = MD5Encrypt(psw);
        }
        return form.ToString();
    }

    private void Login(string result)
    {
        var res = JsonConvert.DeserializeObject<JObject>(result);
        if (res?["status"]?.ToString() == "1")
        {
            var claims = new[]
            {
                new Claim("user_id",res["user_id"]?.ToString()!),
                new Claim("token",res["token"]?.ToString()!),
                new Claim("third_party",result),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1), // 过期时间 1天
                AllowRefresh = true // 如果 AllowRefresh 为 false，不延长 cookie 在客户端计算机硬盘上的保留时间，时间到了客户端计算机就自动删除 cookie
            };
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).Wait();
        }
    }

    private void Logout(string result)
    {
        var res = JsonConvert.DeserializeObject<JObject>(result);
        if (res?["status"]?.ToString() == "1")
        {
            HttpContext.SignOutAsync().Wait();
        }
    }
}