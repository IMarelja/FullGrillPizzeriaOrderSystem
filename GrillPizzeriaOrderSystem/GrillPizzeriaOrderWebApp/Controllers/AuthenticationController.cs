using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    public class AuthenticationController : Controller
{
    private readonly IAuthenticationGrillService _authenticationService;

    public AuthenticationController(IAuthenticationGrillService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginUser)
    {
        if (!ModelState.IsValid)
            return View(loginUser);

        var result = await _authenticationService.LoginAsync(loginUser);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(loginUser);
        }

        Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,   // send only over HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.UserProfile.id.ToString()),
            new Claim(ClaimTypes.Name, result.UserProfile.username),
            new Claim(ClaimTypes.Role, result.UserProfile.roleName),
            new Claim("JWT", result.Token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = loginUser.RememberMe });
        
        if (string.IsNullOrEmpty(loginUser.ReturnUrl) || !Url.IsLocalUrl(loginUser.ReturnUrl))
            return RedirectToAction("Index", "Home");
        
        return LocalRedirect(loginUser.ReturnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null) =>
        View(new RegisterViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerUser)
    {
        if (!ModelState.IsValid)
            return View(registerUser);

        var result = await _authenticationService.RegisterAsync(registerUser);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(registerUser);
        }

        return RedirectToAction("Login", new { returnUrl = registerUser.ReturnUrl });
    }

    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public IActionResult _ChangePassword() => PartialView();

    [HttpPost]
    [Authorize(Policy = "UserOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> _ChangePassword(ChangePasswordViewModel request)
    {
        if (!ModelState.IsValid)
        {
            AuthenticationResult badState = new AuthenticationResult
            {
                Success = false,
                Message = "Invalid model state"
            };

            return PartialView(badState);
        }

        var result = await _authenticationService.ChangePasswordAsync(request);

        return PartialView(result);
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
}
