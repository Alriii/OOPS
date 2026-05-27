namespace MiniEcoMarket
{
    public class Customer : User
    {
        public List<Order> OrderHistory { get; private set; } = new List<Order>();

        public Customer(string name) : base(name) { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Customer] Name: {Name}");
        }

        public void BuyProduct(List<Product> products, List<Order> allOrders)
        {
            try
            {
                Console.Write("Enter Product ID: ");
                int id = int.Parse(Console.ReadLine());

                Product product = products.Find(p => p.Id == id);
                if (product == null)
                    throw new Exception("Product not found!");

                Console.Write($"Enter quantity (Available: {product.Stock}): ");
                int qty = int.Parse(Console.ReadLine());

                if (qty > product.Stock || qty <= 0)
                    throw new Exception("Invalid quantity or insufficient stock!");

                product.Stock -= qty;
                decimal total = product.Price * qty;

                Order order = new Order
                {
                    OrderId = allOrders.Count + 1,
                    CustomerName = Name,
                    ProductName = product.Name,
                    Quantity = qty,
                    TotalPrice = total
                };

                allOrders.Add(order);
                OrderHistory.Add(order);

                Console.WriteLine($"Purchase successful! Total: ₱{total}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RestoreOrder(Order order)
        {
            if (!OrderHistory.Exists(o => o.OrderId == order.OrderId))
                OrderHistory.Add(order);
        }

        public void ViewOrderHistory()
        {
            Console.WriteLine($"\n=== Order History for {Name} ===");
            if (OrderHistory.Count == 0)
            {
                Console.WriteLine("No orders yet.");
                return;
            }

            foreach (var order in OrderHistory)
            {
                order.DisplayInfo();
            }
        }
    }
}