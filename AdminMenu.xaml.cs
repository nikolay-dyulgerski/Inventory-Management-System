using Microsoft.Win32;
using Senior_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Senior_Project_WPF
{
    /// <summary>
    /// Interaction logic for AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Page
    {
        public AdminMenu()
        {
            InitializeComponent();
        }
        private void manageProducts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new manageProductsPage());
        } private void saleAnalytics_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SaleAnalytics());
        }
        private void generateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Sales> sales = Sales.LoadSales();
                if(sales.Count == 0)
                {
                    MessageBox.Show("No sales data found.");
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = $"Sales Report_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Save Sales Report"

                };
                if (saveFileDialog.ShowDialog() != true) return;

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Product Name, Quantity, Total Price, Sale Date");
                foreach(var sale in sales)
                {
                    double cost = Products.GetProductCost(sale.ProductName);
                    double selling = Products.GetProductSellingPrice(sale.ProductName);
                    double grossProfit = (selling - cost) * sale.Quantity;
                    string line = $"{sale.ProductName}, {sale.Quantity}, {sale.TotalPrice:F2},{sale.SaleDate:yyyy-MM-dd},{grossProfit:F2}";
                    double totalRevenue = sales.Sum(s => s.TotalPrice);
                    double totalCost = sales.Sum(s => s.Quantity * Products.GetProductCost(s.ProductName));
                    double totalProfit = totalRevenue - totalCost;
                    csv.AppendLine(line);
                    csv.AppendLine($"Total Revenue,{totalRevenue:F2}");
                    csv.AppendLine($"Total Cost,{totalCost:F2}");
                    csv.AppendLine($"Total Profit,{totalProfit:F2}");
                }
                File.WriteAllText(saveFileDialog.FileName, csv.ToString());
                MessageBox.Show($"Report Generated Successfully: \n{saveFileDialog.FileName}", "Report Generated");
                System.Diagnostics.Process.Start("explorer.exe", saveFileDialog.FileName);
            }catch( Exception ex)
            {
                MessageBox.Show($"Failed to generate report: \n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void predictSales_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new predictSales());
        }
        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
