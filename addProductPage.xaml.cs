using Senior_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Senior_Project_WPF
{
    public partial class addProductPage : Page
    {
        private Products editingProduct;

        public addProductPage(Products productToEdit = null)
        {
            InitializeComponent();

            if (productToEdit != null)
            {
                editingProduct = productToEdit;
                nameInput.Text = productToEdit.Name;
                costPriceInput.Text = productToEdit.costPrice.ToString();
                markupInput.Text = productToEdit.markUp.ToString();
                quantityInput.Text = productToEdit.Quantity.ToString();
                saveButton.Content = "Update Product";
                ShowBarcode(productToEdit.Name);
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            costError.Visibility = double.TryParse(costPriceInput.Text, out double cost) ? Visibility.Collapsed : Visibility.Visible;
            valid &= costError.Visibility == Visibility.Collapsed;

            markError.Visibility = double.TryParse(markupInput.Text, out double markUp) ? Visibility.Collapsed : Visibility.Visible;
            valid &= markError.Visibility == Visibility.Collapsed;

            quantityError.Visibility = int.TryParse(quantityInput.Text, out int quantity) ? Visibility.Collapsed : Visibility.Visible;
            valid &= quantityError.Visibility == Visibility.Collapsed;

            if (!valid)
                return;

            string name = nameInput.Text.Trim();
            List<Products> products = Products.LoadProducts();

            if (editingProduct != null)
            {
                var oldProduct = products.FirstOrDefault(p => p.Name.Equals(editingProduct.Name, StringComparison.OrdinalIgnoreCase));
                if (oldProduct != null)
                {
                    oldProduct.Name = name;
                    oldProduct.costPrice = cost;
                    oldProduct.markUp = markUp;
                    oldProduct.sellingPrice = cost + (cost * markUp / 100);
                    oldProduct.Quantity = quantity;
                }
                Products.SaveProduct(products);
                MessageBox.Show("Product updated successfully!");
                NavigationService.Navigate(new manageProductsPage(name));
            }
            else
            {
                bool nameExists = products.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (nameExists)
                {
                    MessageBox.Show("Product with this name already exists!");
                    return;
                }

                Products newProduct = new Products(name, cost, markUp, quantity);
                products.Add(newProduct);
                Products.SaveProduct(products);
                MessageBox.Show("Product added successfully!");
                NavigationService.Navigate(new manageProductsPage(name));
            }
        }

        private void ShowBarcode(string name)
        {
            string barcodePath = Products.GenerateBarcodeImage(name);
            if (System.IO.File.Exists(barcodePath))
            {
                barcodeImage.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(barcodePath)));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMenu());
        }
    }
}
