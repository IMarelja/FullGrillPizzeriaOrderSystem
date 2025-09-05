using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
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

    [HttpPost]
    [Authorize(Policy = "UserOnly")]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel request)
    {
        try
        {

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .Select(ms => new
                        {
                            Field = ms.Key,
                            Errors = ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        });

                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors
                    });
                }

                var responce = await _authenticationService.ChangePasswordAsync(request);

            if(responce == null)
                return NotFound(new { success = false, message = "Change password request failed or unauthorized" });

            if (!responce.Succeeded)
                return BadRequest(new { success = responce.Succeeded + " " + responce.Message });

            return Ok(new
            {
                success = responce.Succeeded,
                message = responce.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Error with the Password change: " + ex.Message
            });
        }
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("Cart");
        return RedirectToAction("Index", "Home");
    }
}
}
