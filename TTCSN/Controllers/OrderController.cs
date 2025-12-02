using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Models.Order;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderControllerRepository _orderController;
        private readonly OrderDetailControllerRepository _orderDetailController;
        private readonly ProductControllerRepository _productController;
        private readonly ILogger<OrderController> _logger;
        private readonly UserControllerRepository _userController;


        public OrderController(ILogger<OrderController> logger
            ,UserControllerRepository userController
            ,OrderControllerRepository orderController
            ,ProductControllerRepository productController
            ,OrderDetailControllerRepository orderDetailController)
        {
            _orderDetailController = orderDetailController;
            _productController = productController;
            _logger = logger;
            _userController = userController;
            _orderController = orderController;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(OrderStatus? status,
            PaymentMethods? paymentMethods,
            int userId,
            DateTime? startDate,
            DateTime? endDate,
            string? sortBy,
            bool sortDescending = false,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var list = await _orderController.GetOrdersAsync(
                status,
                startDate,
                endDate,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize

            );
            var totalItems = await _orderController.CountOrdersAsync(
                status,
                startDate,
                endDate
            );

            var orders = new OrderListViewModel
            {
                Orders = (await Task.WhenAll(
                    list.Select(async o => new OrderViewModel
                    {
                        Id = o.Id,
                        Customer = await _userController.GetInfoUser(o.UserId),
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        PaymentMethod = o.PaymentMethod,
                        Address = o.Address,
                        PhoneNumber = o.PhoneNumber,
                        Note = o.Note
                    })
                )).ToList(),
                SortBy = sortBy,
                SortDescending = sortDescending,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            return View(orders);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderDetails = await _orderController.GetOrderDetailsByOrderIdAsync(orderId);
            var orderStatus = await _orderController.GetStatusOrderById(orderId);
            var orderDetailViewModels = new OrderDetailListViewModel()
            {
                OrderDetails = (await Task.WhenAll(orderDetails.Select(async od => new OrderDetailViewModel
                {
                    Id = od.Id,
                    OrderId = orderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    ImageUrl = od.ImageUrl,
                    ProductName = od.ProductName,

                    IsReviewed = (await _orderDetailController.GetIsReviewed(orderId, od.ProductId)) == 1,
                    IsDelivered = (await _orderDetailController.GetIsDelivered(orderId, od.ProductId)) == 1

                }))).ToList()
            };
            _logger.LogInformation("check delivered {a}", orderDetailViewModels.OrderDetails.FirstOrDefault()?.IsDelivered);
            _logger.LogInformation("check reviewed {a} ", orderDetailViewModels.OrderDetails.FirstOrDefault()?.IsReviewed);
            return View(orderDetailViewModels);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status, string? note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var success = await _orderController.UpdateOrderStatusAsync(orderId, status, note);
            if (status == OrderStatus.Cancelled)
            {
                var orderDetails = await _orderController.GetOrderDetailsByOrderIdAsync(orderId);
                foreach (var item in orderDetails)
                {
                    var check = await _productController.IncreaseProductStockAsync(item.ProductId, item.Quantity);
                    if (!check)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Cập nhật trạng thái đơn hàng thất bại"
                        });
                    }
                }
            }
            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = "Cập nhật trạng thái đơn hàng thất bại"
                });
            }
            if (status == OrderStatus.Delivered)
            {
            foreach (var item in await _orderController.GetOrderDetailsByOrderIdAsync(orderId))
                {
                    var check = await _orderDetailController.UpdateIsDelivered(orderId, item.ProductId);
                    if (!check)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Cập nhật trạng thái đơn hàng thất bại"
                        });
                    }
                }            
            }
            return Json(new
            {
                success = true,
                message = "Cập nhật trạng thái đơn hàng thành công"
            });
        }
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var orders = await _orderController.GetOrdersByUserId(userId);
            foreach(var order in orders)
            {
                _logger.LogInformation("check status {a}", order.Status);
            }
            var orderViewModels = new OrderListViewModel()
            {
                Orders = orders.Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    Address = o.Address,
                    PhoneNumber = o.PhoneNumber,
                    Note = o.Note

                }).ToList()
            };
            return View(orderViewModels);
        }
        

    }

}
