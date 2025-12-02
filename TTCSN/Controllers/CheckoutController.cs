using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Services;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly OrderDetailControllerRepository _orderDetailRepo;
        private readonly OrderControllerRepository _orderRepo;
        private readonly CartService _cartService;
        private readonly ProductControllerRepository _proRepo;
        private readonly UserControllerRepository _userRepo;
        private readonly ILogger<CheckoutController> _logger;
        
        public CheckoutController(ProductControllerRepository proRepo
            , UserControllerRepository userRepo
            , ILogger<CheckoutController> logger
            , CartService cartService
            , OrderControllerRepository orderRepo
            , OrderDetailControllerRepository orderDetailRepo)
        {
            _userRepo = userRepo;
            _proRepo = proRepo;
            _logger = logger;
            _cartService = cartService;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartService.GetCartDetailsAsync();
            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var cartItems = await _cartService.GetCartDetailsAsync();
            foreach (var (product, quantity) in cartItems)
            {
                var checkStock = await _proRepo.CheckProductStockAsync(product.Id, quantity);
                if (!checkStock)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {product.Name} không đủ số lượng trong kho.";
                    return RedirectToAction("PaymentFailed");
                }
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var address = User.FindFirstValue("Address");
            var phone = User.FindFirstValue(ClaimTypes.MobilePhone) ?? "";
            if (string.IsNullOrWhiteSpace(address))
            {
                TempData["ErrorMessage"] = "Vui lòng cập nhật địa chỉ giao hàng trong thông tin cá nhân.";
                return RedirectToAction("PaymentFailed");
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                TempData["ErrorMessage"] = "Vui lòng cập nhật số điện thoại trong thông tin cá nhân.";
                return RedirectToAction("PaymentFailed");
            }
            var newOrder = await _orderRepo.CreateNewOrder(new Order()
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                PaymentMethod = (PaymentMethods)paymentMethod,
                TotalAmount = 0, // This should be calculated based on cart items
                Address = address,
                PhoneNumber = phone
            });
            if (newOrder != null)
            {            
                foreach (var (product, quantity) in cartItems)
                {
                    await _proRepo.ReduceProductStockAsync(product.Id, quantity);
                    await _orderDetailRepo.AddOrderDetail(new OrderDetail()
                    {
                        OrderId = newOrder.Id,
                        ProductId = product.Id,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl
                    });
                }

                decimal totalAmount = _cartService.GetCartTotalPrice(cartItems);
                var isUpdated = await _orderRepo.UpdateOrderTotalAmountAsync(newOrder.Id, totalAmount);
                if (isUpdated)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("OrderSuccess");
                }
                TempData["ErrorMessage"] =  "Lỗi không cập nhật được tổng tiền đơn hàng";
                await _orderRepo.DeleteOrderAsync(newOrder.Id);
                return RedirectToAction("PaymentFailed");
            }
            TempData["ErrorMessage"] = "Không tạo được đơn hàng";
            return RedirectToAction("PaymentFailed");
        }
        public IActionResult PaymentFailed()
        {
            return View();
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
