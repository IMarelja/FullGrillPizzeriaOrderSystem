using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelConstants;
using ViewModels;

namespace GrillPizzeriaOrderWebApp.Controllers
{

    public class OrderController : Controller
    {
        private const string CartCookieName = "Cart";

        public OrderController()
        {
        }

        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public IActionResult AddItem(int foodId)
        {
            if (foodId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid food or quantity." });
            }

            // Read existing cart from cookie (if any)
            OrderCreateViewModel? cart = null;

            if (Request.Cookies.TryGetValue(CartCookieName, out var existing) && !string.IsNullOrWhiteSpace(existing))
            {
                // Cookie exists and has content - deserialize it
                try
                {
                    cart = JsonSerializer.Deserialize<OrderCreateViewModel>(existing);
                }
                catch (Exception)
                {
                    // If deserialization fails, treat as if no valid cart exists
                    cart = null;
                }
            }

            // New cart if none exists
            if (cart == null)
                cart = new OrderCreateViewModel
                {
                    items = new List<OrderItemCreateViewModel>()
                };
            

            // Upsert item
            var item = cart.items.FirstOrDefault(i => i.foodId == foodId);

            if (item == null)
            {
                cart.items.Add(new OrderItemCreateViewModel
                {
                    foodId = foodId,
                    quantity = 1
                });
            }
            else
            {
                var newQty = item.quantity + 1;
                item.quantity = Math.Min(ValidationConstants.QuantityOfFoodMax, Math.Max(ValidationConstants.QuantityOfFoodMin, newQty));
            }

            // Write updated cart back to cookie
            var json = JsonSerializer.Serialize(cart);

            Response.Cookies.Append(
                CartCookieName,
                json,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

            var totalCount = cart.items.Sum(i => i.quantity);

            return Ok(new { success = true, count = totalCount });
        }

        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public IActionResult RemoveItem(int foodId)
        {
            if (foodId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid food or quantity." });
            }
            // Read existing cart from cookie (if any)
            OrderCreateViewModel? cart = null;
            if (Request.Cookies.TryGetValue(CartCookieName, out var existing) && !string.IsNullOrWhiteSpace(existing))
            {
                // Cookie exists and has content - deserialize it
                try
                {
                    cart = JsonSerializer.Deserialize<OrderCreateViewModel>(existing);
                }
                catch (Exception)
                {
                    // If deserialization fails, treat as if no valid cart exists
                    cart = null;
                }
            }
            // If no cart or no items, nothing to remove
            if (cart == null || cart.items == null || !cart.items.Any())
            {
                return BadRequest(new { success = false, message = "Cart is empty." });
            }
            // Find item
            var item = cart.items.FirstOrDefault(i => i.foodId == foodId);
            if (item == null)
            {
                return BadRequest(new { success = false, message = "Item not found in cart." });
            }
            // Decrease quantity or remove item
            if (item.quantity > 1)
            {
                var newQty = item.quantity - 1;
                item.quantity = Math.Min(ValidationConstants.QuantityOfFoodMax, Math.Max(ValidationConstants.QuantityOfFoodMin, newQty));
            }
            else
            {
                cart.items.Remove(item);
            }
            // Write updated cart back to cookie or delete if empty
            if (cart.items.Any())
            {
                var json = JsonSerializer.Serialize(cart);
                Response.Cookies.Append(
                    CartCookieName,
                    json,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
            }
            else
            {
                Response.Cookies.Delete(CartCookieName);
            }
            var totalCount = cart.items.Sum(i => i.quantity);
            return Ok(new { success = true, count = totalCount });
        }

        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> CreateOrder()
        {
            if(!Request.Cookies.TryGetValue(CartCookieName, out var existing) || string.IsNullOrWhiteSpace(existing))
                return BadRequest(new { success = false, message = "Cart is empty" });

            OrderCreateViewModel? cart = JsonSerializer.Deserialize<OrderCreateViewModel>(existing);

            if(cart == null || cart.items == null || !cart.items.Any())
                return BadRequest(new { success = false, message = "Cart is empty" });



            Response.Cookies.Delete(CartCookieName);
            return Ok(new { success = true });
        }
    }
}
