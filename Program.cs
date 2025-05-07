using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Senior_Project { 

    class Program
    {
        static void Main(string[] args)
        {               //OPENING
            Console.WriteLine("Welcome to the Sales and Analytics Management System! ");
            Console.Write("1. Register\n 2. Log in");
            int choice;
           while(!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            {
                Console.WriteLine("Invalid Input. Enter 1(register), 2(log in): ");
            }
            //Register or Log in Credentials
            if(choice == 1)
            {
                RegisterUser();                

            } else if(choice == 2)
            {
                User loggedInUser = LoginUser();
                if (loggedInUser != null) {
                    Console.WriteLine($"Log in Successfull! Welcome, {loggedInUser.Username} ({loggedInUser.Role})");
                    ShowRoleBasedMenu(loggedInUser);
                }
                else
                {
                    Console.WriteLine("Invalid Credentials. Try Again.");
                }
            } else
            {
                Console.WriteLine("Invalid Option. Exiting...");
            }



        }
        static void RegisterUser()
        {
            String username;
            do { Console.Write("Enter username: ");
                username = Console.ReadLine()?.Trim(); } while(string.IsNullOrEmpty(username));

            String password;
            do { Console.Write("Enter password: ");
                password = Console.ReadLine()?.Trim(); } while (string.IsNullOrEmpty(password));
            String role;
            do
            {
                Console.Write("Enter role: Admin/Employee: ");
                role = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(role))
                {
                    role = char.ToUpper(role[0]) + role.Substring(1).ToLower();
                }
            }  while (role != "Admin" && role != "Employee");
            User newUser = role switch
            {
                "Admin" => new Admin(username, password),
                "Employee" => new Employee(username, password),
                _ => throw new InvalidOperationException("Unsupported role")
            };

            newUser.RegisterUser();
            Console.WriteLine("Successful Registration! "); 
            User loggedInUser = User.LoginUser(username, password);
            if(loggedInUser != null) { Console.WriteLine($"Log in Successfull! Welcome, {loggedInUser.Username} ({loggedInUser.Role})");
                ShowRoleBasedMenu(loggedInUser);
            }


        }
        static User LoginUser()
        {
            Console.WriteLine("Enter username: ");
            String username = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Enter password: "); 
            String password = Convert.ToString(Console.ReadLine()) ;  
            return User.LoginUser(username, password);  
        }
        static void ShowRoleBasedMenu(User user)
        {
            bool running = true;
            while (running)
            {
                if(user.Role == "Admin")
                {
                    Console.WriteLine("Admin Menu!");
                    Console.WriteLine("1. Manage Products");
                    Console.WriteLine("2. View Sales Analytics ");
                    Console.WriteLine("3. Generate Report ");
                    Console.WriteLine("4. Predict Sales");
                    Console.WriteLine("5. View Urgent Restocking Products");
                    Console.WriteLine("6. Log out");
                    Console.WriteLine("Select an option(1-5): ");
                    string choice = Console.ReadLine();
                    switch(choice)
                    {
                        case "1":
                            Products.AdminMenu();
                            break;
                        case "2": 
                            Sales.DisplaySales();
                            break;
                        case "3":
                            Sales.GenerateReport();
                            break;
                        case "4":
                            Console.WriteLine("Sales prediction mode! ");
                            Console.WriteLine("1. Simulate Prediction: ");
                            Console.WriteLine("2. Use real-life data: ");
                            int dataChoice = Convert.ToInt32(Console.ReadLine());
                            List<Sales> dataSource = new();
                            if (dataChoice == 1)
                            {
                                
                                dataSource = new List<Sales>
                                {
                                      new Sales("Phone", 10, 10 * Products.GetProductSellingPrice("Phone"), new DateTime(2025, 3, 1)),
                                      new Sales("Phone", 12, 12 * Products.GetProductSellingPrice("Phone"), new DateTime(2025, 3, 2)),
                                      new Sales("Phone", 8, 8 * Products.GetProductSellingPrice("Phone"), new DateTime(2025, 3, 3)),
                                      new Sales("Phone", 15, 15 * Products.GetProductSellingPrice("Phone"), new DateTime(2025, 3, 4)),
                                      new Sales("Phone", 11, 11 * Products.GetProductSellingPrice("Phone"), new DateTime(2025, 3, 5)),
    
                                       
                                      new Sales("Laptop", 5, 5 * Products.GetProductSellingPrice("Laptop"), new DateTime(2025, 3, 1)),
                                      new Sales("Laptop", 7, 7 * Products.GetProductSellingPrice("Laptop"), new DateTime(2025, 3, 2)),
                                      new Sales("Laptop", 6, 6 * Products.GetProductSellingPrice("Laptop"), new DateTime(2025, 3, 3)),
                                      new Sales("Laptop", 8, 8 * Products.GetProductSellingPrice("Laptop"), new DateTime(2025, 3, 4)),
                                      new Sales("Laptop", 4, 4 * Products.GetProductSellingPrice("Laptop"), new DateTime(2025, 3, 5)),
    
                                        
                                      new Sales("Tablet", 9, 9 * Products.GetProductSellingPrice("Tablet"), new DateTime(2025, 3, 1)),
                                      new Sales("Tablet", 11, 11 * Products.GetProductSellingPrice("Tablet"), new DateTime(2025, 3, 2)),
                                      new Sales("Tablet", 10, 10 * Products.GetProductSellingPrice("Tablet"), new DateTime(2025, 3, 3)),
                                      new Sales("Tablet", 13, 13 * Products.GetProductSellingPrice("Tablet"), new DateTime(2025, 3, 4)),
                                      new Sales("Tablet", 12, 12 * Products.GetProductSellingPrice("Tablet"), new DateTime(2025, 3, 5))


                                };
                                
                            }
                            else if( dataChoice == 2)
                            {

                                dataSource = Sales.LoadSales();
                            }
                            else
                            {
                                Console.WriteLine("Invalid option. Choose between 1 and 2. ");
                            }
                            Console.WriteLine("Choose prediction type: ");
                            Console.WriteLine("1.Predict best-selling product ");
                            Console.WriteLine("2. Predict sales for a specific product");
                            Console.WriteLine("Choice: ");
                            int predictionType = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Choose number of days to predict: ");
                            int days = Convert.ToInt32(Console.ReadLine());
                            if(predictionType == 1)
                            {
                                var predictions = SalesPrediction.PredictAllProducts(dataSource, days);
                                Console.WriteLine($"Predicted sales for the next {days} days: ");
                                foreach(var pair in predictions.OrderByDescending(p=> p.Value))
                                    Console.WriteLine($" - {pair.Key}: {pair.Value:F0} units");
                                Console.WriteLine("Moving Average Predictions: ");
                                foreach(var group in dataSource.GroupBy(s => s.ProductName))
                                {
                                    var salesList = group.OrderBy(s=> s.SaleDate).Select(s=>s.Quantity).ToList();   
                                    if (salesList != null && salesList.Count >= 3)
                                    {
                                        var movingAvg = new MovingAveragePrediction(salesList);
                                        List<double> movingAvgPrediction = movingAvg.PredictDays(3, days);
                                       for (int i = 0; i < movingAvgPrediction.Count; i++)
                                        {
                                            Console.WriteLine($" - {group.Key} (Day {i+1}): {movingAvgPrediction[i]:F0} units (in Moving Average)");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($" - {group.Key}: Not enough data for Moving Average");
                                    }
                                }
                                var top = predictions.OrderByDescending(p => p.Value).FirstOrDefault();
                                Console.WriteLine($"\n Best-selling product predicted: {top.Key} with {top.Value:F0} units");

                            }else if(predictionType == 2)
                            {
                                Console.WriteLine("Enter product name: ");
                                string productName = Console.ReadLine();
                                double regressionPrediction = SalesPrediction.PredictSalesUsingData(dataSource, productName, days);
                                var productSales = dataSource
                                    .Where(s=> s.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                                    .OrderBy(s=> s.SaleDate)
                                    .Select(s=>s.Quantity)
                                    .ToList();
                                double movingAvgPrediction = 0;
                                if(productSales.Count >= 3)
                                {
                                    var movingAvg = new MovingAveragePrediction (productSales);
                                    List<double> MovingAvgPrediction = movingAvg.PredictDays(3, days);
                                    Console.WriteLine($"Predicted sales for {productName} (Linear Regression): {regressionPrediction:F0}");
                                    for(int i = 0; i < MovingAvgPrediction.Count; i++)
                                    {
                                        Console.WriteLine($"Predicted sales for {productName} (Moving Average Day {i + 1}): {MovingAvgPrediction[i]:F0}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Not enough data for Moving Average Prediction.");
                                }
                                Console.WriteLine($"Predicted sales for {productName} (Linear Regression): {regressionPrediction:F0} ");
                                Console.WriteLine($"Predicted sales for {productName} (Moving Average): {movingAvgPrediction:F0} ");
                            }
                            else
                            {
                                Console.WriteLine("Invalid Choice.");
                            }

                            break;
                        case "5":
                            PrintUrgentRestocks();
                            break;
                        case "6": Console.WriteLine("");
                            running = false;
                            break;
                        default: Console.WriteLine("Invalid Option. Choose from 1-5"); 
                                break;
                    }
                }else if(user.Role == "Employee")
                    {
                    Console.WriteLine("Employee Menu!");
                    Console.WriteLine("1.Add sales record");
                    Console.WriteLine("2.View Sales");
                    Console.WriteLine("3.Log out");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Sales.RecordSale();
                            break;
                        case "2":
                            Sales.DisplaySales();
                            break;
                        case "3":
                            Console.WriteLine("Logging out ");
                            running = false;
                            break;
                        default: Console.WriteLine("Invalid Option choose from 1 or 2");
                            break;
                    }

                }
            }
        }
        static void PrintUrgentRestocks()
        {
            var urgentProducts = RestockManager.GetUrgentProducts();
            if(urgentProducts.Count == 0)
            {
                Console.WriteLine("No urgent restocking needed.");
                return;
            }
            Console.WriteLine("Urgent Restocking: ");
            foreach(var urgentProduct in RestockManager.GetUrgentProducts())
            {
                Console.WriteLine($"- {urgentProduct.ProductName}: {urgentProduct.RemainingQuantity} units left.");
            }
        }
    }



} 
