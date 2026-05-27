namespace MiniEcoMarket
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine($"Order #{OrderId} | {ProductName} x{Quantity} | Total: ₱{TotalPrice} | Customer: {CustomerName}");
        }
    }
}