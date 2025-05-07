using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
namespace Senior_Project
{
    public class Products
    {
        private static readonly string FilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.dat");
        public string Name { get; set; }
        public double costPrice { get; set; }
        public double markUp { get; set; }
        public double sellingPrice { get; set; }
        public int Quantity { get; set; }
        public string BarcodePath { get; set; }
    
        public Products(string name, double costPrice, double markUp, int quantity)
        {
            this.Name = name;
            this.costPrice = costPrice;
            this.markUp = markUp;
            this.Quantity = quantity;
            this.sellingPrice = costPrice + (costPrice * markUp / 100); // calculation for selling price 
            this.BarcodePath = GenerateBarcodeImage(name);

        }
        private static readonly bool[][] EAN13_LeftEncoding = {
    new bool[] { false, false, false, true, true, false, true },  
    new bool[] { false, false, true, true, false, false, true },  
    new bool[] { false, false, true, false, false, true, true },  
    new bool[] { false, true, true, true, true, false, true },    
    new bool[] { false, true, false, false, false, true, true },  
    new bool[] { false, true, true, false, false, false, true },  
    new bool[] { false, true, false, true, true, true, true },    
    new bool[] { false, true, true, true, false, true, true },    
    new bool[] { false, true, true, false, true, true, true },    
    new bool[] { false, false, false, true, false, true, true }   
};

        private static readonly bool[][] EAN13_RightEncoding = {
    new bool[] { true, true, true, false, false, true, false },  
    new bool[] { true, true, false, false, true, true, false },  
    new bool[] { true, true, false, true, true, false, false },  
    new bool[] { true, false, false, false, false, true, false },  
    new bool[] { true, false, true, true, true, false, false },  
    new bool[] { true, false, false, true, true, true, false },  
    new bool[] { true, false, true, false, false, false, false },  
    new bool[] { true, false, false, false, true, false, false },  
    new bool[] { true, false, false, true, false, false, false },  
    new bool[] { true, true, true, false, true, false, false }   
};

    public static string GenerateBarcodeImage(string productName)
        {
          string barcode = GenerateBarcodeNumber();
          string folderPath = "barcodes";
          string path = Path.Combine(folderPath, $"{productName}.png");
          if (!Directory.Exists(folderPath))
          {
            Directory.CreateDirectory(folderPath);
            LogActivity.WriteLog($"Created folder {folderPath}");
            }

            int width = 500;
            int height = 200;
            int barHeight = 120;
            int xPosition = 30;
            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", 18, FontStyle.Bold))
            {
                g.Clear(Color.White); 

                g.FillRectangle(Brushes.Black, xPosition, 30,2, barHeight);
                xPosition += 4;
                g.FillRectangle(Brushes.Black, xPosition, 30,2, barHeight);
                xPosition += 4;
                for (int i = 0; i < 6; i++)
                {
                    int digit = barcode[i] - '0';
                    foreach (bool isBar in EAN13_LeftEncoding[digit])
                    {
                        if (isBar)
                            g.FillRectangle(Brushes.Black, xPosition, 30, 3, barHeight);
                        xPosition += 3;
                    }
                    xPosition += 2;
                }
                g.FillRectangle(Brushes.Black, xPosition, 30, 2, barHeight);
                xPosition += 4;
                g.FillRectangle(Brushes.Black, xPosition, 30, 2, barHeight);
                xPosition += 4;
                for (int i = 6; i < 13; i++)
                {
                    int digit = barcode[i] - '0';
                    foreach (bool isBar in EAN13_RightEncoding[digit])
                    {
                        if (isBar)
                            g.FillRectangle(Brushes.Black, xPosition, 30, 3, barHeight);
                        xPosition += 3;
                    }
                    xPosition += 2;
                }
                g.FillRectangle(Brushes.Black, xPosition, 30, 2, barHeight);
                xPosition += 4;
                g.FillRectangle(Brushes.Black, xPosition, 30, 2, barHeight);

                g.DrawString(barcode, font, Brushes.Black, new PointF(width/3.2f, height - 50));

                try
                {
                    bitmap.Save(path, ImageFormat.Png);
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error Saving Barcode: {ex.Message}");
                    LogActivity.WriteLog($"Error saving barcode of {productName}, {ex.Message}");
                    return "barcodes/default.png";
                }
            } return path;
        }

        public static string GenerateBarcodeNumber()
        {
            Random rand = new Random();
            string barcode = "100";
            for (int i = 0; i < 9; i++)
                barcode += rand.Next(0, 10).ToString();
            barcode += CalculateEAN13Checksum(barcode);
            return barcode;
        }
        public static string CalculateEAN13Checksum(string code)
        {
            int sum = 0;    
            for(int i =0; i <12; i++)
            {
                int digit = int.Parse(code[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }
            int checksum = (10 - (sum % 10) % 10);
            return checksum.ToString();
        }
        public static bool ProductExists(string productName)
        {
            return LoadProducts().Any(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }
        public static int GetProductQuantity(string productName)
        {
            var product = LoadProducts().FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            return product != null ? product.Quantity : 0;
        }
        public static double GetProductSellingPrice(string productName)
        {
            var product = LoadProducts().FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            return product != null ? product.sellingPrice : 0;
        }
        public static void UpdateProductQuantity(string productName, int quantityChange)
        {
            List<Products> products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if (product != null)
            {
                product.Quantity += quantityChange;
                SaveProduct(products);
                LogActivity.WriteLog($"Updated Product Quantity: {productName} ");
            }
        }
        public void SaveProduct()
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(Name);
                writer.Write(costPrice);
                writer.Write(markUp);
                writer.Write(sellingPrice);
                writer.Write(Quantity);
                writer.Write(BarcodePath ?? "barcodes/default.png");
                LogActivity.WriteLog($"Saved Product Successfully, {Name}");
            }
        }
        public static List<Products> LoadProducts()
        {
            List<Products> products = new List<Products>();
            if (!File.Exists(FilePath)) return products;
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        try
                        {
                            string name = reader.ReadString();
                            double costPrice = reader.ReadDouble();
                            double markUp = reader.ReadDouble();
                            double sellingPrice = reader.ReadDouble();
                            int quantity = reader.ReadInt32();
                            string barcodePath = reader.ReadString();
                            if (string.IsNullOrWhiteSpace(name) || quantity < 0)
                            {
                                Console.WriteLine("Skipping invalid product record.");
                                continue;
                            }
                            products.Add(new Products(name, costPrice, markUp, quantity){
                                BarcodePath = barcodePath
                            });
                        }
                        
                        catch (EndOfStreamException){
                            Console.WriteLine("Incomplete product record found.");
                            LogActivity.WriteLog($"Incomlpete porduct record: {products}");
                            break;
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"Error reading data: {ex.Message}");
                            LogActivity.WriteLog($"Error reading data, {ex.Message}");
                        }
                    }
                }
                return products;
            }
        }
        public static void DisplayProducts()
        {
            List<Products> products = LoadProducts();
            if (products.Count == 0)
            {
                Console.WriteLine("There are not products to display");
                LogActivity.WriteLog("No Products Displayed. ");
                return;
            }
            Console.WriteLine("Product List: ");
            foreach (var product in products)
            {
                Console.WriteLine($"Name: {product.Name}, Cost Price: {product.costPrice}, MarkUp: {product.markUp}%, Selling Price: {product.sellingPrice}, Quantity: {product.Quantity} ");
                Console.WriteLine($"Barcode Image: {product.BarcodePath}");
                LogActivity.WriteLog($"Displayed Product: {product.Name}");
            }
        }
        public static void SaveProduct(List<Products> products)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using(BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach(var product in products)
                {
                    writer.Write(product.Name);
                    writer.Write(product.costPrice);
                    writer.Write(product.markUp);
                    writer.Write(product.sellingPrice);
                    writer.Write(product.Quantity);
                    writer.Write(product.BarcodePath ?? "barcodes/default.png");
                    LogActivity.WriteLog($"Saved Product: {product.Name}");
                }
            }
        }
        public static double GetProductCost(string productName)
        {
            List<Products> products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            return product != null ? product.costPrice : 0;
        }
        public static void UpdateDeleteProduct(string productName)
        {

            List<Products> products = LoadProducts();
            Products productToEdit = products.Find(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if(productToEdit == null)
            {
                Console.WriteLine("Product not found");
                LogActivity.WriteLog("Could not find the product");
                return;
            }
            Console.WriteLine("Update or delete a product? ");
            Console.WriteLine("1. Update product");
            Console.WriteLine("2. Delete Product");
            Console.WriteLine("Enter choice: ");
            string choice = Console.ReadLine(); 
            if(choice == "1")
            {
                LogActivity.WriteLog(choice);
                Console.WriteLine("What do you want to edit ? ");
                Console.WriteLine("1. Product name");
                Console.WriteLine("2. Cost Price");
                Console.WriteLine("3. Mark Up");
                Console.WriteLine("4. Quantity: ");
                Console.WriteLine("Enter option: ");
                int editChoice = Convert.ToInt32(Console.ReadLine());

                switch (editChoice)
                {
                    case 1:
                        Console.WriteLine("Enter new name: ");
                        productToEdit.Name = Console.ReadLine();
                        LogActivity.WriteLog($"Editted product name: {productName} to {productToEdit.Name}");
                        break;
                    case 2:
                        Console.WriteLine("Enter new cost price: ");
                        productToEdit.costPrice = Convert.ToDouble(Console.ReadLine());
                        productToEdit.sellingPrice = productToEdit.costPrice + (productToEdit.markUp * productToEdit.costPrice / 100);
                        LogActivity.WriteLog($"Editted cost price: {productName}");
                        break;
                    case 3:
                        Console.WriteLine("Enter new markup: ");
                        productToEdit.markUp = Convert.ToDouble(Console.ReadLine());
                        productToEdit.sellingPrice = productToEdit.costPrice + (productToEdit.markUp * productToEdit.costPrice / 100);
                        LogActivity.WriteLog($"Editted markup: {productName}");
                        break;
                    case 4:
                        Console.WriteLine("Enter quantity: ");
                        int AdditionalQuantity = Convert.ToInt32(Console.ReadLine());
                        productToEdit.Quantity += AdditionalQuantity;
                        LogActivity.WriteLog($"Editted quantity: {productName} ");
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        LogActivity.WriteLog("Invalid Choice");
                        return;
                     
                }
                SaveProduct(products);
                Console.WriteLine("Product saved successfully!");

            }else if(choice == "2")
            {
                products.Remove(productToEdit);
                SaveProduct(products);
                Console.WriteLine("Product deleted successfully");
                LogActivity.WriteLog($"Deleted Product:{productName}");
            }
            else
            {
                Console.WriteLine("Invalid Choice!");
            }
            LogActivity.WriteLog($"Product '{productToEdit.Name} ' updated. New Quantity  {productToEdit.Quantity}.");


        }
        public static void AdminMenu()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Product Management: ");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. View Products");
                Console.WriteLine("3. Edit Products: ");
                Console.WriteLine("4. Exit");
                Console.WriteLine("Choose Between options: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter Product Name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter Cost Price: ");
                        double costPrice = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter MarkUp Percentage : ");
                        double markUp = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter Quantity: ");
                        int quantity = Convert.ToInt32(Console.ReadLine()); 
                        Products newProduct = new Products(name, costPrice, markUp,quantity);
                        newProduct.SaveProduct();
                        Console.WriteLine("Product added successfully! ");
                        LogActivity.WriteLog($"Added Product: {name}");
                        break;
                    case 2:
                        DisplayProducts();
                        break;
                    case 3:
                        Console.WriteLine("Enter product name to edit: ");
                        string updateName = Console.ReadLine();
                        UpdateDeleteProduct(updateName);
                        break;
                    case 4:
                        running = false;
                        LogActivity.WriteLog($"Exited");
                        break;
                    default:
                        Console.WriteLine("Invalid Option try again! ");
                        LogActivity.WriteLog("User entered invalid option");
                        break;

                }
            }


        }
    }
}
