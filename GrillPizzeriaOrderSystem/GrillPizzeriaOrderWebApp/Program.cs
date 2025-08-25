using System.Net.Http.Headers;
using GrillPizzeriaOrderWebApp.Services.APIs;
using GrillPizzeriaOrderWebApp.Services.IAPIs;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Role policies (admin/ user)

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("admin"));
    options.AddPolicy("UserOnly", p => p.RequireRole("user", "admin"));
});


// HttpClients per resource
var baseUrl = builder.Configuration.GetValue<string>("WebAPI:BaseUrl");


builder.Services.AddHttpClient("AuthenticationAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl + "Authentication/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("DataAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
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
