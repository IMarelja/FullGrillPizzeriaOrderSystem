using System.Net.Http.Headers;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.APIs;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NuGet.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Role policies (admin/ user)

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("admin"));
    options.AddPolicy("UserOnly", p => p.RequireRole("user", "admin"));
});

builder.Services.Configure<WebAPISettings>(builder.Configuration.GetSection("WebAPI"));


// Register IHttpContextAccessor to access current user's token
builder.Services.AddHttpContextAccessor();

// HttpClients per resource

builder.Services.AddHttpClient<IAuthenticationGrillService, AuthenticationRepository>((serviceProvider, client) =>
{
    var webApiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(webApiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IFoodService, FoodRepository>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IFoodCategoryService, FoodCategoryRepository>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IAllergenService, AllergenRepository>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IUserService, UserRepository>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

});

builder.Services.AddHttpClient<ILogService, LogRepository>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<WebAPISettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "text/plain");

});

// Cookies authentication

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.LoginPath = "/Authentication/Login";
        opts.LogoutPath = "/Authentication/Logout";
        opts.AccessDeniedPath = "/Authentication/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
