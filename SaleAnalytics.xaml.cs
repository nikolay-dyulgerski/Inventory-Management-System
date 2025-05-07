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
    /// Interaction logic for SaleAnalytics.xaml
    /// </summary>
    public partial class SaleAnalytics : Page
    {
        public SaleAnalytics()
        {
            InitializeComponent();
            LoadSalesData();
        }
        private void LoadSalesData()
        {
            List<Sales> sales = Sales.LoadSales();
            if (sales.Count == 0)
            {
                MessageBox.Show("No sales data available yet.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            salesDataGrid.ItemsSource = sales;
            double totalRevenue = sales.Sum(s => s.TotalPrice);
            totalRevenueText.Text = $"Total Revenue: ${totalRevenue:F2}";
            DrawBarChart(sales);
            DrawLineChart(sales);
        }
        private void DrawBarChart(List<Sales> sales)
        {
            barChartCanvas.Children.Clear();
            var grouped = sales
                .GroupBy(s=> s.ProductName)
                .Select(g => new {Product = g.Key, Quantity = g.Sum( s=> s.Quantity ) })
                .OrderByDescending(g=>g.Quantity)
                .Take(10)
                .ToList();
            double maxQuantity = grouped.Max(g=> g.Quantity);   
            double canvasHeight = barChartCanvas.ActualHeight > 0 ? barChartCanvas.ActualHeight : 200;
            double canvasWidth = barChartCanvas.ActualWidth > 0 ? barChartCanvas.ActualWidth : 600;
            double barWidth = canvasWidth / grouped.Count;
            for (int i = 0; i < grouped.Count; i++)
            {
                var data = grouped[i];
                double barHeight = (data.Quantity / maxQuantity) * (canvasHeight - 40);  
                Rectangle rect = new Rectangle
                {
                    Width = barWidth - 10,
                    Height = barHeight,
                    Fill = Brushes.SteelBlue

                };
                Canvas.SetLeft(rect, i * barWidth + 5);
                Canvas.SetTop(rect, canvasHeight - barHeight);
                barChartCanvas.Children.Add(rect);
                TextBlock label = new TextBlock
                {
                    Text = data.Product,
                    Width = barWidth,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 10,
                    Foreground = Brushes.Black
                    
                };
                Canvas.SetLeft(label, i * barWidth);
                Canvas.SetTop(label, canvasHeight - 20);
                barChartCanvas.Children.Add(label);

            }

        }
        private void DrawLineChart(List<Sales> sales)
        {
            lineChartCanvas.Children.Clear();
            var daily = sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(s => s.TotalPrice) })
                .OrderBy(g => g.Date)
                .ToList();
            if (daily.Count < 2) return;
            double maxRevenue = daily.Max(d => d.Revenue);
            double canvasHeight = lineChartCanvas.ActualHeight > 0 ? lineChartCanvas.ActualHeight : 200;
            double canvasWidth = lineChartCanvas.ActualWidth > 0 ? lineChartCanvas.ActualWidth : 600;
            double xStep = canvasWidth / (daily.Count - 1);
            Polyline line = new Polyline
            {
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2
            };
            for (int i = 0; i < daily.Count; i++)
            {
                double x = i * xStep;
                double y = canvasHeight - (daily[i].Revenue / maxRevenue) * (canvasHeight - 40); 
                line.Points.Add(new Point(x, y));
                Ellipse dot = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.DarkGreen
                };
                Canvas.SetLeft(dot, x - 3);
                Canvas.SetTop(dot, y - 3);
                lineChartCanvas.Children.Add(dot);

                TextBlock datelabel = new TextBlock
                {
                    Text = daily[i].Date.ToString("MM - dd"),
                    FontSize = 10,
                    Width = xStep,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(datelabel, x - xStep/2.5);
                Canvas.SetTop(datelabel, canvasHeight - 1);
                lineChartCanvas.Children.Add(datelabel);
            }
            lineChartCanvas.Children.Add(line);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
