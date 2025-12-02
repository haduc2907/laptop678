using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TTCSN.Models;
using TTCSN.Services;

namespace TTCSN.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _cartService.GetCartDetailsAsync();
            var cartItems = new CartListViewModel()
            {
                Items = list.Select(ci => new CartViewModel
                {
                    ProductId = ci.product.Id,
                    ProductName = ci.product.Name,
                    Price = ci.product.Price,
                    Quantity = ci.quantity,
                    ImageUrl = ci.product.ImageUrl
                }).ToList()
            };
            ViewBag.Total = _cartService.GetCartTotalPrice(list);
            return View(cartItems);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            _cartService.AddToCart(productId, quantity);
            return Json(new
            {
                success = true,
                message = "Đã thêm vào giỏ hàng",
                cartCount = _cartService.GetCartItemCount()
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var succsess = _cartService.UpdateCartItem(productId, quantity);
            if (!succsess)
            {
                return Json(new
                {
                    success = false,
                    message = "Cập nhật giỏ hàng thất bại"
                });
            }
            var cartItems = await _cartService.GetCartDetailsAsync();
            return Json(new
            {
                success = true,
                message = "Cập nhật giỏ hàng thành công",
                cartCount = _cartService.GetCartItemCount(),
                toltal = _cartService.GetCartTotalPrice(cartItems)
            });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            var cartItems = await _cartService.GetCartDetailsAsync();
            return Json(new
            {
                success = true,
                cartCount = _cartService.GetCartItemCount(),
                total = _cartService.GetCartTotalPrice(cartItems)
            });
        }

        // GET: Số lượng sản phẩm (cho header icon)
        public IActionResult GetCartCount()
        {
            return Json(_cartService.GetCartItemCount());
        }
    }
}
