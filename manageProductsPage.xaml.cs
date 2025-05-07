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
    /// Interaction logic for manageProductsPage.xaml
    /// </summary>
    public partial class manageProductsPage : Page
    {
        private List<Products> products;
        public manageProductsPage(string highlightProductName = null)
        {
            InitializeComponent();
            products = Products.LoadProducts();
            productsDataGrid.ItemsSource = products;

            if (!string.IsNullOrEmpty(highlightProductName))
            {
                var index = products.FindIndex(p => p.Name == highlightProductName);
                if (index >= 0)
                {
                    productsDataGrid.SelectedIndex = index;
                    productsDataGrid.ScrollIntoView(products[index]);
                }
            }
        }

        private void LoadAndBindProducts()
        {
            List<Products> products = Products.LoadProducts();
            productsDataGrid.ItemsSource = products;
        }
        private void viewProduct_Click(object sender, RoutedEventArgs e)
        {
            LoadAndBindProducts();
        }
        
        private void addProductClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new addProductPage());
        }
        private void editSelected_Click(object sender, RoutedEventArgs e)
        {
            if(productsDataGrid.SelectedItem is Products selected)
            {
                NavigationService.Navigate(new editProduct(selected));
            }
            else
            {
                MessageBox.Show("Select a product to edit.");
            }
        }
        private void deleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if( productsDataGrid.SelectedItem is Products selected)
            {
                var result = MessageBox.Show($"Delete product '{selected.Name}'?", "Confirm,", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    products.RemoveAll(p=>p.Name == selected.Name);
                    Products.SaveProduct(products);
                    productsDataGrid.ItemsSource = null;
                    productsDataGrid.ItemsSource = products;

                }
            }
            else
            {
                MessageBox.Show("Select a products to delete.");
            }
        }
        private void urgentRestock_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UrgentRestock());
        }

        private void BackButon_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMenu());
        }
    }
}
