using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GrillPizzeriaOrderWebApp.Models;
using GrillPizzeriaOrderWebApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{
    
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public  IActionResult Index()
            => View();

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var user = await _userService.GetMeAsync();

                if(user == null)
                    return NotFound(new { success = false, message = "User information failed to load or unauthorized" });

                return Ok(new
                {
                    success = true,
                    data = user,
                    message = "User loaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error loading user info: " + ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateMe(UserUpdateViewModel userinfo)
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
                        message = "Invalid User request",
                        errors
                    });
                }

                var user = await _userService.UpdateMe(userinfo);

                return Ok(new
                {
                    success = user.Succeeded,
                    data = user,
                    message = user.Message + " " + string.Join(", ", user.Errors),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error updating user info: " + ex.Message
                });
            }
        }
    }
}
