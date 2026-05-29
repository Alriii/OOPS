using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

namespace MiniEcoMarket
{
    class Program
    {
        static List<Product> products = new List<Product>();
        static List<Order> orders = new List<Order>();
        static List<User> users = new List<User>();

       
        // NOTE: user inputs should not include the '|' character.
        const string ProductsFile = "products.txt";
        const string OrdersFile = "orders.txt";
        const string UsersFile = "users.txt";

        static void Main(string[] args)
        {
            LoadData();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== MINI ECOMARKET - Farm to Table ===");
                Console.WriteLine("1. Register as Farmer");
                Console.WriteLine("2. Register as Customer");
                Console.WriteLine("3. Login as Farmer");
                Console.WriteLine("4. Login as Customer");
                Console.WriteLine("5. View All Products");
                Console.WriteLine("6. Exit");
                Console.Write("Choose option: ");

                try
                {
                    int choice = int.Parse(Console.ReadLine() ?? "");

                    switch (choice)
                    {
                        case 1: RegisterFarmer(); break;
                        case 2: RegisterCustomer(); break;
                        case 3: FarmerMenu(); break;
                        case 4: CustomerMenu(); break;
                        case 5: ViewAllProducts(); break;
                        case 6: SaveData(); running = false; break;
                        default: Console.WriteLine("Invalid option!"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void LoadData()
        {
            products.Clear();
            orders.Clear();
            users.Clear();

            if (File.Exists(ProductsFile))
            {
                foreach (var line in File.ReadAllLines(ProductsFile))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Id|Name|Price|Stock|Category|FarmerName
                    var parts = line.Split('|');
                    if (parts.Length < 6) continue;

                    products.Add(new Product
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Price = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                        Stock = int.Parse(parts[3]),
                        Category = parts[4],
                        FarmerName = parts[5]
                    });
                }

                int nextId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
                Farmer.SetNextProductId(nextId);
            }

            if (File.Exists(OrdersFile))
            {
                foreach (var line in File.ReadAllLines(OrdersFile))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // OrderId|CustomerName|ProductName|Quantity|TotalPrice
                    var parts = line.Split('|');
                    if (parts.Length < 5) continue;

                    orders.Add(new Order
                    {
                        OrderId = int.Parse(parts[0]),
                        CustomerName = parts[1],
                        ProductName = parts[2],
                        Quantity = int.Parse(parts[3]),
                        TotalPrice = decimal.Parse(parts[4], CultureInfo.InvariantCulture)
                    });
                }
            }

            if (File.Exists(UsersFile))
            {
                foreach (var line in File.ReadAllLines(UsersFile))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Type|Name   where Type is "Farmer" or "Customer"
                    var parts = line.Split('|');
                    if (parts.Length < 2) continue;

                    var type = parts[0];
                    var name = parts[1];

                    if (type == "Farmer")
                        users.Add(new Farmer(name));
                    else if (type == "Customer")
                        users.Add(new Customer(name));
                }
            }

            foreach (var order in orders)
            {
                var customer = users.OfType<Customer>()
                    .FirstOrDefault(c => c.Name.Equals(order.CustomerName, StringComparison.OrdinalIgnoreCase));
                customer?.RestoreOrder(order);
            }
        }

        static void SaveData()
        {
            // Products
            using (var sw = new StreamWriter(ProductsFile, false))
            {
                foreach (var p in products)
                {
                    // Id|Name|Price|Stock|Category|FarmerName
                    sw.WriteLine(string.Join("|",
                        p.Id.ToString(),
                        p.Name ?? "",
                        p.Price.ToString(CultureInfo.InvariantCulture),
                        p.Stock.ToString(),
                        p.Category ?? "",
                        p.FarmerName ?? ""
                    ));
                }
            }

            // Orders
            using (var sw = new StreamWriter(OrdersFile, false))
            {
                foreach (var o in orders)
                {
                    // OrderId|CustomerName|ProductName|Quantity|TotalPrice
                    sw.WriteLine(string.Join("|",
                        o.OrderId.ToString(),
                        o.CustomerName ?? "",
                        o.ProductName ?? "",
                        o.Quantity.ToString(),
                        o.TotalPrice.ToString(CultureInfo.InvariantCulture)
                    ));
                }
            }

            // Users
            using (var sw = new StreamWriter(UsersFile, false))
            {
                foreach (var u in users)
                {
                    var type = u is Farmer ? "Farmer" : "Customer";
                    sw.WriteLine($"{type}|{u.Name ?? ""}");
                }
            }

            Console.WriteLine("Data saved. Goodbye!");
        }

        static void RegisterFarmer()
        {
            Console.Write("Enter your name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            if (users.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("That name is already registered.");
                return;
            }

            users.Add(new Farmer(name));
            Console.WriteLine("Farmer registered successfully!");
        }

        static void RegisterCustomer()
        {
            Console.Write("Enter your name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }

            if (users.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("That name is already registered.");
                return;
            }

            users.Add(new Customer(name));
            Console.WriteLine("Customer registered successfully!");
        }

        static Farmer? LoginFarmer()
        {
            Console.Write("Enter farmer name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return null;
            }

            var farmer = users.OfType<Farmer>()
                .FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (farmer == null)
                Console.WriteLine("Farmer not found. Register first.");

            return farmer;
        }

        static Customer? LoginCustomer()
        {
            Console.Write("Enter customer name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return null;
            }

            var customer = users.OfType<Customer>()
                .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (customer == null)
                Console.WriteLine("Customer not found. Register first.");

            return customer;
        }

        static void FarmerMenu()
        {
            Farmer? farmer = LoginFarmer();
            if (farmer == null)
                return;

            bool inMenu = true;
            while (inMenu)
            {
                Console.Clear();
                farmer.DisplayInfo();
                Console.WriteLine("\n=== Farmer Menu ===");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. View My Products");
                Console.WriteLine("3. Remove Product");
                Console.WriteLine("4. Logout");
                Console.Write("Choose option: ");

                try
                {
                    int choice = int.Parse(Console.ReadLine() ?? "");

                    switch (choice)
                    {
                        case 1:
                            farmer.AddProduct(products);
                            SaveData();
                            break;
                        case 2:
                            farmer.ViewMyProducts(products);
                            break;
                        case 3:
                            farmer.RemoveProduct(products);
                            SaveData();
                            break;
                        case 4:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                if (inMenu)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CustomerMenu()
        {
            Customer? customer = LoginCustomer();
            if (customer == null)
                return;

            bool inMenu = true;
            while (inMenu)
            {
                Console.Clear();
                customer.DisplayInfo();
                Console.WriteLine("\n=== Customer Menu ===");
                Console.WriteLine("1. Buy Product");
                Console.WriteLine("2. View Order History");
                Console.WriteLine("3. Logout");
                Console.Write("Choose option: ");

                try
                {
                    int choice = int.Parse(Console.ReadLine() ?? "");

                    switch (choice)
                    {
                        case 1:
                            ViewAllProducts();
                            customer.BuyProduct(products, orders);
                            SaveData();
                            break;
                        case 2:
                            customer.ViewOrderHistory();
                            break;
                        case 3:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                if (inMenu)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void ViewAllProducts()
        {
            Console.WriteLine("\n=== All Products ===");
            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }

            foreach (var product in products)
                product.DisplayInfo();
        }

        class UserRecord
        {
            public string Type { get; set; } = "";
            public string Name { get; set; } = "";
        }
    }
}
