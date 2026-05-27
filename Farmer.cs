namespace MiniEcoMarket
{
    public class Farmer : User
    {
        private static int productCounter = 1;

        public static void SetNextProductId(int nextId) => productCounter = nextId;

        public Farmer(string name) : base(name) { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Farmer] Name: {Name}");
        }

        public void AddProduct(List<Product> products)
        {
            Console.Write("Product Name: ");
            string name = Console.ReadLine();
            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine());
            Console.Write("Stock: ");
            int stock = int.Parse(Console.ReadLine());
            Console.Write("Category: ");
            string category = Console.ReadLine();

            Product product = new Product
            {
                Id = productCounter++,
                Name = name,
                Price = price,
                Stock = stock,
                Category = category,
                FarmerName = Name
            };

            products.Add(product);
            Console.WriteLine("Product added successfully!");
        }

        public void ViewMyProducts(List<Product> products)
        {
            Console.WriteLine($"\n=== Products by {Name} ===");
            bool hasProducts = false;
            foreach (var p in products)
            {
                if (p.FarmerName == Name)
                {
                    p.DisplayInfo();
                    hasProducts = true;
                }
            }
            if (!hasProducts) Console.WriteLine("No products listed yet.");
        }

        public void RemoveProduct(List<Product> products)
        {
            Console.WriteLine($"\n=== Remove Product ({Name}) ===");

            var myProducts = products.Where(p => p.FarmerName == Name).ToList();
            if (myProducts.Count == 0)
            {
                Console.WriteLine("No products available to remove.");
                return;
            }

            Console.WriteLine("Your products:");
            foreach (var p in myProducts)
                p.DisplayInfo();

            Console.Write("Enter Product ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Product ID.");
                return;
            }

            var product = products.FirstOrDefault(p => p.Id == id && p.FarmerName == Name);
            if (product == null)
            {
                Console.WriteLine("Product not found (or it's not yours).");
                return;
            }

            products.Remove(product);
            Console.WriteLine("Product removed successfully!");
        }
    }
}