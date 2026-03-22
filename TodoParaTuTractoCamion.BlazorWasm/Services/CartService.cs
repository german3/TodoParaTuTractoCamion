using System.Net.Http.Json;
using TodoParaTuTractoCamion.Application.DTOs;

namespace TodoParaTuTractoCamion.BlazorWasm.Services
{
    public class CartService
    {
        private readonly HttpClient _http;
        private List<CartItem> _items = new List<CartItem>();

        public event Action OnChange;

        public CartService(HttpClient http)
        {
            _http = http;
        }

        public IReadOnlyList<CartItem> GetItems() => _items.AsReadOnly();

        public async Task<string> AddToCart(ProductoDto producto, int cantidad)
        {
            if (producto.Stock < cantidad)
                return $"No hay suficiente stock para {producto.Nombre}.";

            var existingItem = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (existingItem != null)
            {
                if (producto.Stock < existingItem.Cantidad + cantidad)
                    return $"No hay suficiente stock para {producto.Nombre}.";
                
                existingItem.Cantidad += cantidad;
            }
            else
            {
                _items.Add(new CartItem { Producto = producto, Cantidad = cantidad });
            }

            NotifyStateChanged();
            return null;
        }

        public void RemoveFromCart(Guid productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                _items.Remove(item);
                NotifyStateChanged();
            }
        }

        public decimal GetTotal() => _items.Sum(i => i.Producto.Precio * i.Cantidad);

        public void AdjustQuantity(Guid productoId, int delta)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item == null) return;

            item.Cantidad += delta;
            if (item.Cantidad <= 0)
                _items.Remove(item);

            NotifyStateChanged();
        }


        public async Task<bool> ConfirmPurchase()
        {
            var command = new { Items = _items.Select(i => new { ProductoId = i.Producto.Id, Cantidad = i.Cantidad }).ToList() };
            var response = await _http.PostAsJsonAsync("api/compras", command);
            
            if (response.IsSuccessStatusCode)
            {
                _items.Clear();
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    public class CartItem
    {
        public ProductoDto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}
