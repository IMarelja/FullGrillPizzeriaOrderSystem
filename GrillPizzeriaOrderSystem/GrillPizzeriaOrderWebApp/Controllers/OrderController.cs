using System.Text.Json;
using GrillPizzeriaOrderWebApp.Services.IServices;
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
        private readonly IOrderService _orderService;
        private readonly IFoodService _foodService;

        public OrderController(IOrderService orderService, IFoodService foodService)
        {
            _orderService = orderService;
            _foodService = foodService;
        }

        [Authorize(Policy = "UserOnly")]
        public IActionResult Index()
            => View();

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetCart()
        {
            OrderCreateViewModel? cart = null;
            if (Request.Cookies.TryGetValue(CartCookieName, out var existing) && !string.IsNullOrWhiteSpace(existing))
            {
                try
                {
                    cart = JsonSerializer.Deserialize<OrderCreateViewModel>(existing);
                }
                catch (Exception)
                {
                    cart = null;
                }
            }
            if (cart == null || cart.items == null || !cart.items.Any())
            {
                return Ok(new { success = true, items = new List<object>(), totalPrice = 0.0m });
            }
            var detailedItems = new List<object>();
            decimal totalPrice = 0.0m;
            foreach (var item in cart.items)
            {
                var food = await _foodService.GetById(item.foodId);
                if (food != null)
                {
                    var itemTotal = food.price * item.quantity;
                    totalPrice += itemTotal;
                    detailedItems.Add(new
                    {
                        foodId = item.foodId,
                        foodName = food.name,
                        quantity = item.quantity,
                        unitPrice = food.price,
                        totalPrice = itemTotal,
                        allergen = food.allergens.Select(a => a.name).ToList(), // Could not have any allergens
                        category = food.category.name
                    });
                }
            }
            return Ok(new { success = true, items = detailedItems, totalPrice });
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetUserOrdersAsync();

            var model = orders?.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                OrderTotalPrice = o.OrderTotalPrice,
                Items = o.Items.Select(oi => new OrderItemViewModel
                {
                    Quantity = oi.Quantity,
                    LineTotal = oi.LineTotal,
                    Food = new FoodViewModel
                    {
                        id = oi.Food.id,
                        name = oi.Food.name,
                        description = oi.Food.description,
                        price = oi.Food.price,
                        imagePath = oi.Food.imagePath,

                        // map category
                        category = new CategoryFoodViewModel
                        {
                            id = oi.Food.category.id,
                            name = oi.Food.category.name
                        },

                        // map allergens
                        allergens = oi.Food.allergens
                            .Select(a => new AllergenViewModel
                            {
                                id = a.id,
                                name = a.name
                            })
                            .ToList()
                    }
                }).ToList()
            }).ToList() ?? new List<OrderViewModel>();


            return PartialView("_SentOrders", model);
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


            var responce = await _orderService.CreateAsync(cart);

            if (!responce.Succeeded)
                return BadRequest(new { success = false, message = responce.Message ?? "Order creation failed." });

            Response.Cookies.Delete(CartCookieName);
            return Ok(new { success = true });
        }
    }
}
