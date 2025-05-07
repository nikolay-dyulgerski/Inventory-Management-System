using System;
using System.Collections.Generic;
using System.Linq;
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
using Senior_Project;

namespace Senior_Project_WPF
{
    /// <summary>
    /// Interaction logic for EmployeeMenu.xaml
    /// </summary>
    public partial class EmployeeMenu : Page
    {
        public EmployeeMenu()
        {
            InitializeComponent();
            LoadProducts();
            LoadSales();
        }
        private List<string> allProductNames = new List<string>();
        private void LoadProducts()
        {
            var products = Products.LoadProducts();
            allProductNames = products.Select(p=>p.Name).ToList();
        }
        private void LoadSales()
        {
            var sales = Sales.LoadSales();
            salesDataGrid.ItemsSource = sales;
        }
        private void RecordSale_Click(object sender, RoutedEventArgs e)
        {
            string productName = productSearchBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(productName))
            {
                saleStatusText.Text = "Please enter a product name.";
                saleStatusText.Foreground = Brushes.Red;
                return;
            }
            if (!int.TryParse(quantityInputBox.Text.Trim(), out int quantity) || quantity <= 0)
            {
                saleStatusText.Text = "Enter a valid quantity.";
                saleStatusText.Foreground = Brushes.Red;
                return;
            }
            MessageBoxResult confirm = MessageBox.Show($"Confirm sale of {quantity} units of {productName} ?", 
                "Confirm Sale", MessageBoxButton.YesNo,MessageBoxImage.Question);    
            if(confirm!= MessageBoxResult.Yes)
            {
                saleStatusText.Text = "Sale was canceled";
                saleStatusText.Foreground= Brushes.OrangeRed;
                return;
            }
            bool success;
            string result = Sales.RecordSale_wpf(productName, quantity, out success);
            saleStatusText.Text = result;
            saleStatusText.Foreground = success ? Brushes.DarkGreen : Brushes.Red;
            if(success)
            {
                productSearchBox.Clear();
                quantityInputBox.Clear();
                LoadSales();
            }
        }
        private void productSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = productSearchBox.Text.Trim().ToLower();
            if(string.IsNullOrEmpty(input))
            {
                productSuggestions.Visibility = Visibility.Collapsed;
                return;
            }
            var suggestions = allProductNames
                .Where(name => name.ToLower().Contains(input))
                .Take(5)
                .ToList();
            productSuggestions.ItemsSource = suggestions;
            productSuggestions.Visibility = suggestions.Any() ? Visibility.Visible : Visibility.Collapsed;  
        }
        private void productSuggestions_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(productSuggestions.SelectedItem is string selected)
            {
                productSearchBox.Text = selected;
                productSuggestions.Visibility = Visibility.Collapsed;
            }
        }
        private void LogOut_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
