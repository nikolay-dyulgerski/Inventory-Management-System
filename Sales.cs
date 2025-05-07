using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Senior_Project
{
    public class Sales
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "sales.dat");
        public string ProductName { get; set; }
        public int Quantity {  get; set; }
        public double TotalPrice {  get ; set; }
        public DateTime SaleDate {  get; set; }

        public Sales(string productName, int quantity, double totalPrice)
        {
           this.ProductName = productName;
           this.Quantity = quantity;
           this.TotalPrice = totalPrice;
           this.SaleDate = DateTime.Now;
        }
        public Sales(string productName, int quantity, double totalPrice, DateTime saleDate)
        {
            this.ProductName = productName;
            this.Quantity = quantity;
            this.TotalPrice = totalPrice;
            this.SaleDate = saleDate;
        }
        public void SaveSales()
        {
            using(FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None)) 
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(ProductName);
                writer.Write(Quantity);
                writer.Write(TotalPrice);
                writer.Write(SaleDate.ToBinary());
            }
        }
        public static List<Sales> LoadSales() { 

            List<Sales> sales = new List<Sales>();
            if (!File.Exists(FilePath))  return sales;
            using(FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read,FileShare.Read))
            using(BinaryReader reader = new BinaryReader(fs))
            {
                while(reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string productName = reader.ReadString();
                    int quantity = reader.ReadInt32(); 
                    double totalPrice = reader.ReadDouble();
                    DateTime salesDate = DateTime.FromBinary(reader.ReadInt64());
                    sales.Add(new Sales(productName, quantity, totalPrice, salesDate));
                }
                LogActivity.WriteLog($"Loaded {sales.Count}");
            } return sales;
        }
        public static bool CheckStockMessage(string productName, int quantity, out string message)
        {
            int available = Products.GetProductQuantity(productName);
            message = "";

            if (quantity > available)
            {
                message = $"Not enough stock. Available stock: {available}";
                return false;
            }

            int remaining = available - quantity;
            if (remaining <= 0)
            {
                message = $"Alert: {productName} was sold. No more units left!";
            }
            else if (remaining <= 3)
            {
                message = $"Alert: {productName} is low in stock! Only {remaining} units will remain!";
            }

            return true;
        }

        public static void RecordSale()
        {
            Console.WriteLine("Enter name: ");
            string productName = Console.ReadLine();
            LogActivity.WriteLog($"Product was recorded {productName} ");

           

            Console.WriteLine("Enter Quantity: ");
            if(!int.TryParse(Console.ReadLine(), out int quantity)|| quantity <= 0)
            {
                Console.WriteLine("Invalid Quantity entered");
                return;
            }
            bool success;
            string result = RecordSale_wpf(productName, quantity, out success);
            Console.WriteLine(result);
            if (success)
            {
                RestockManager.MonitorStockAfterSale(productName);
            }
            LogActivity.WriteLog($"[Console Sale] {result}");

        }
        public static string RecordSale_wpf(string productName, int quantity, out bool success)
        {
            success = false;
            if(!Products.ProductExists(productName)) return "Product does not exist,";
            if(!CheckStockMessage(productName,quantity, out string stockMessage)) return stockMessage;
            double totalPrice = quantity * Products.GetProductSellingPrice(productName);
            var sale = new Sales(productName,quantity,totalPrice);
            sale.SaveSales();
            Products.UpdateProductQuantity(productName, -quantity);
            RestockManager.MonitorStockAfterSale(productName);
            success = true;
            return $"Sale recorded: {productName}, Quantity: {quantity}";
        }


        public static void DisplaySales()
        {
            List<Sales> sales = LoadSales();
            if(sales.Count == 0)
            {
                Console.WriteLine("No records of sales!");
                LogActivity.WriteLog("Tried to display sales, no record of them was found. ");
                return;
            }
            Console.WriteLine("Sales Record: ");
            foreach(var sale in sales)
            {
                Console.WriteLine($"Product: {sale.ProductName}, Quantity: {sale.Quantity}, Total Price: {sale.TotalPrice}, Date Time: {sale.SaleDate} ");
                LogActivity.WriteLog("Displayed sales successfuly! ");
            }

        }
       public static void GenerateReport()
        {
            List<Sales> sales = LoadSales();
            if(sales.Count == 0)
            {
                Console.WriteLine("No data recorded!");
                LogActivity.WriteLog("Tried to generate report, no data was recorded");
            }
            double totalRevenue = sales.Sum(s => s.TotalPrice); 
            double totalCost = sales.Sum(s => s.Quantity * Products.GetProductCost(s.ProductName));
            double grossProfit = totalRevenue - totalCost;

            Console.WriteLine("Sales Analytics: ");
            Console.WriteLine("Total Revenue: " + totalRevenue);
            Console.WriteLine("Total cost: " + totalCost);
            Console.WriteLine("Gross Profit: " + grossProfit);
            LogActivity.WriteLog("Generated Report Successfully! ");

            var bestSellingProduct = sales.GroupBy(s => s.ProductName)
                                          .OrderByDescending(g => g.Sum(s => s.Quantity))
                                          .FirstOrDefault();
            if(bestSellingProduct != null)
            {
                Console.WriteLine($"Most sold product: {bestSellingProduct.Key}\n Total sold: {bestSellingProduct.Sum(s=> s.Quantity)}");
                LogActivity.WriteLog($"Displayed most sold product: '{bestSellingProduct}'");
            }
        }
    }
}
