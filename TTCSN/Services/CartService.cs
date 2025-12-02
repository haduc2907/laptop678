using TTCSN.Entities;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartItems";
        private readonly ProductControllerRepository _proRepo;
        public CartService(ProductControllerRepository proRepo, IHttpContextAccessor httpContextAccessor)
        {
            _proRepo = proRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return new List<Entities.CartItem>();
            }
            var cartItems = Helpers.SessionExtensions.GetObjectFromJson<List<Entities.CartItem>>(session, CartSessionKey);
            return cartItems ?? new List<Entities.CartItem>();
        }
        public void SaveCartItems(List<CartItem> cartItems)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                Helpers.SessionExtensions.SetObjectAsJson(session, CartSessionKey, cartItems);
            }
        }
        public void AddToCart(int productId, int quantity = 1)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(ci => ci.productId == productId);
            if (existingItem != null)
            {
                existingItem.quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem { productId = productId, quantity = quantity });
            }
            SaveCartItems(cartItems);
        }
        public void RemoveFromCart(int productId)
        {
            var cartItems = GetCartItems();
            var itemToRemove = cartItems.FirstOrDefault(ci => ci.productId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                SaveCartItems(cartItems);
            }
        }
        public  void ClearCartAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(CartSessionKey);
            }
        }
        public async Task<List<(Product product, int quantity)>> GetCartDetailsAsync()
        {
            var cartItems = GetCartItems();
            var cartDetails = new List<(Product product, int quantity)>();
            foreach (var item in cartItems)
            {
                var product = await _proRepo.GetProductById(item.productId);
                if (product != null)
                {
                    cartDetails.Add((product, item.quantity));
                }
            }
            return cartDetails;
        }
        public decimal GetCartTotalPrice(List<(Product product, int quantity)> cartDetails)
        {
            decimal total = 0;
            foreach (var (product, quantity) in cartDetails)
            {
                total += product.Price * quantity;
            }
            return total;
        }
        public int GetCartItemCount()
        {
            var cartItems = GetCartItems();
            return cartItems.Sum(ci => ci.quantity);
        }
        public bool UpdateCartItem(int productId, int quantity)
        {
            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(ci => ci.productId == productId);
            if (existingItem != null)
            {
                if (quantity <= 0)
                {
                    cartItems.Remove(existingItem);
                }
                else
                {
                    existingItem.quantity = quantity;
                }
                SaveCartItems(cartItems);
                return true;
            }
            return false;
        }
    }
}
