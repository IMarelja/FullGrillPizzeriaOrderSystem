using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly IHttpClientFactory _http;
        public AuthenticationController(IHttpClientFactory http) => _http = http;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null) =>
            View(new LoginViewModel { ReturnUrl = returnUrl });
        public async Task<IActionResult> Login(LoginViewModel loginUser)
        {
            if (!ModelState.IsValid)
                return View(loginUser);

            var authClient = _http.CreateClient("AuthenticationAPI");
            var authResp = await authClient.PostAsJsonAsync("login", new { loginUser.Username, loginUser.Password });
            var authRaw = await authResp.Content.ReadAsStringAsync();

            if (!authResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Login failed {(int)authResp.StatusCode}: {authRaw}");
                return View(loginUser);
            }

            var authData = JsonSerializer.Deserialize<Dictionary<string, string>>(authRaw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (authData == null || !authData.TryGetValue("token", out var jwt))
            {
                ModelState.AddModelError(string.Empty, "Invalid login response.");
                return View(loginUser);
            }
            
            var dataClient = _http.CreateClient("DataAPI");
            dataClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var profileResp = await dataClient.GetAsync("User/");
            if (!profileResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Failed to load profile: {profileResp.StatusCode}");
                return View(loginUser);
            }

            var profileRaw = await profileResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(profileRaw);
            var root = doc.RootElement;

            var userId = root.GetProperty("id").GetInt32().ToString();
            var username = root.GetProperty("username").GetString();
            var role = root.GetProperty("roleName").GetString();
            
            var claims = new List<Claim>
            {
                
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("JWT", jwt)
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
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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

            var client = _http.CreateClient("AuthenticationAPI");
            var resp = await client.PostAsJsonAsync("register", new
            {
                registerUser.Username,
                registerUser.FirstName,
                registerUser.LastName,
                registerUser.Phone,
                registerUser.Email,
                registerUser.Password,
                registerUser.ConfirmPassword
            });

            var raw = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Registration failed {(int)resp.StatusCode}: {raw}");
                return View(registerUser);
            }

            return RedirectToAction("Login", new { returnUrl = registerUser.ReturnUrl });
        }
    }
}
