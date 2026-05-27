namespace MiniEcoMarket
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public string FarmerName { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id} | {Name} | ₱{Price} | Stock: {Stock} | {Category} | Farmer: {FarmerName}");
        }
    }
}