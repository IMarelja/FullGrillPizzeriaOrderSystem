using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(loginUser);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.UserProfile.Id.ToString()),
            new Claim(ClaimTypes.Name, result.UserProfile.Username),
            new Claim(ClaimTypes.Role, result.UserProfile.RoleName),
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
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(registerUser);
        }

        return RedirectToAction("Login", new { returnUrl = registerUser.ReturnUrl });
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _authenticationService.ChangePasswordAsync(request);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(request);
        }

        TempData["SuccessMessage"] = "Password changed successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
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
