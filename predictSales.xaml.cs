using Senior_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Senior_Project_WPF
{
    public partial class predictSales : Page
    {
        private List<Sales> simulatedSales;

        public predictSales()
        {
            InitializeComponent();
            LoadSimulatedData();
            PopulateProducts();
        }

        private void LoadSimulatedData()
        {
            simulatedSales = new List<Sales>
            {
                new Sales("Phone", 10, 100, new DateTime(2025, 3, 1)),
                new Sales("Phone", 12, 120, new DateTime(2025, 3, 2)),
                new Sales("Phone", 8, 80, new DateTime(2025, 3, 3)),
                new Sales("Phone", 15, 150, new DateTime(2025, 3, 4)),
                new Sales("Phone", 11, 110, new DateTime(2025, 3, 5)),
                new Sales("Laptop", 5, 500, new DateTime(2025, 3, 1)),
                new Sales("Laptop", 7, 700, new DateTime(2025, 3, 2)),
                new Sales("Laptop", 6, 600, new DateTime(2025, 3, 3)),
                new Sales("Laptop", 8, 800, new DateTime(2025, 3, 4)),
                new Sales("Laptop", 4, 400, new DateTime(2025, 3, 5)),
                new Sales("Tablet", 9, 900, new DateTime(2025, 3, 1)),
                new Sales("Tablet", 11, 1100, new DateTime(2025, 3, 2)),
                new Sales("Tablet", 10, 1000, new DateTime(2025, 3, 3)),
                new Sales("Tablet", 13, 1300, new DateTime(2025, 3, 4)),
                new Sales("Tablet", 12, 1200, new DateTime(2025, 3, 5))
            };
        }

        private List<Sales> GetSelectedDateSource()
        {
            return realData.IsChecked == true ? Sales.LoadSales() : simulatedSales;
        }

        private void PopulateProducts()
        {
            var products = Products.LoadProducts();  
            var names = products.Select(p => p.Name).Distinct().ToList();
            productCombo.ItemsSource = names;
            if (names.Any()) productCombo.SelectedIndex = 0;
        }

        private void PredictProduct_Click(object sender, EventArgs e)
        {
            var sales = GetSelectedDateSource();
            string product = productCombo.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(product)) return;
            if (!int.TryParse(daysInput.Text, out int days)) days = 30;

            double linearResult = SalesPrediction.PredictSalesUsingData(sales, product, days);

            var productSales = sales
                .Where(s => s.ProductName.Equals(product, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.SaleDate)
                .Select(s => s.Quantity)
                .ToList();

            string movingAverageResult = "Not enough data for Moving Average";

            if (productSales.Count >= 3)
            {
                var movingAvg = new MovingAveragePrediction(productSales);
                List<double> movingPredictions = movingAvg.PredictDays(3, days);
                movingAverageResult = $"{movingPredictions.Sum():F0} units (over {days} days)";
            }

            resultText.Text = $"{product} Sales Prediction for next {days} days:\n" +
                              $"- Linear Regression: {linearResult:F0} units\n" +
                              $"- Moving Average: {movingAverageResult}";
        }

        private void PredictBestSeller_Click(object sender, RoutedEventArgs e)
        {
            var sales = GetSelectedDateSource();
            if (!int.TryParse(daysInput.Text, out int days)) days = 30;

            var predictions = SalesPrediction.PredictAllProducts(sales, days);
            if (!predictions.Any())
            {
                resultText.Text = "Not enough data to predict!";
                return;
            }

            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendLine($"Predicted sales in next {days} days:");

            foreach (var product in predictions.OrderByDescending(p => p.Value))
            {
                var productSales = sales
                    .Where(s => s.ProductName.Equals(product.Key, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(s => s.SaleDate)
                    .Select(s => s.Quantity)
                    .ToList();

                string movingAvgResult = "Not enough data";

                if (productSales.Count >= 3)
                {
                    var movingAvg = new MovingAveragePrediction(productSales);
                    List<double> movingPredictions = movingAvg.PredictDays(3, days);
                    movingAvgResult = $"{movingPredictions.Sum():F0} units";
                }

                resultBuilder.AppendLine($"- {product.Key}: Linear = {product.Value:F0} units, Moving Avg = {movingAvgResult}");
            }

            resultText.Text = resultBuilder.ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
