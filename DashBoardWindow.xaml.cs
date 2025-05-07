using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Senior_Project;


namespace Senior_Project_WPF
{
    public partial class DashBoardWindow : Page
    {
        
        private void DashBoardWindows_Loaded(object sender, RoutedEventArgs e)
        {
            double[] RealSalesData = GetSalesData();
            DrawChart(RealSalesData);
        }
        private double[] GetSalesData()
        {
            List<Sales> salesRecords = Sales.LoadSales();
           
            var groupedData = salesRecords
            .GroupBy(s=> s.SaleDate.Date)
            .Select(g=> new {Date = g.Key, TotalQuantity = g.Sum(s=> s.Quantity)})
            .OrderBy(g => g.Date) 
            .ToList();
            
            
            return groupedData.Select(g=>(double)g.TotalQuantity).ToArray();
        }
       
        private void DrawChart(double[] salesData)
        {
            if(salesData == null || salesData.Length < 2) {

                MessageBox.Show("Not enough sales data Available to display to the chart.");
                return;
            
            }
            
            int pointCount = salesData.Length;

            // Use the ActualWidth and ActualHeight of the canvas; if not set, use default values.
            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;
            if (canvasWidth == 0) canvasWidth = 700;
            if (canvasHeight == 0) canvasHeight = 300;

            double margin = 50;
            double usableWidth = canvasWidth - 2 * margin;
            double usableHeight = canvasHeight - 2 * margin;

            double xInterval = usableWidth / (pointCount - 1);
            double maxSales = salesData.Max();
            double yScale = usableHeight / maxSales;
            chartCanvas.Children.Clear();
            Line yAxis = new Line()
            {
                X1 = margin,
                Y1 = margin,
                X2 = margin,
                Y2 = margin + usableHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            chartCanvas.Children.Add(yAxis);
            Line xAxis = new Line() { X1 = margin,
                Y1 = margin + usableHeight,
                X2 = margin + usableWidth,
                Y2 = margin + usableHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 2 };
            chartCanvas.Children.Add(xAxis);
            int numberofYTicks = 5;
            for (int i = 0; i < numberofYTicks; i++)
            {
                double yPos = margin + usableHeight - (usableHeight * i / numberofYTicks);
                Line yTick = new Line()
                {
                    X1 = margin - 5,
                    Y1 = yPos,
                    X2 = margin,
                    Y2 = yPos,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1

                };
                chartCanvas.Children.Add(yTick);
                double tickValue = maxSales * i / numberofYTicks;
                TextBlock yLabel = new TextBlock()
                {
                    Text = tickValue.ToString("F0"),
                    FontSize = 12
                };
                Canvas.SetLeft(yLabel, 30);
                Canvas.SetTop(yLabel, yPos - 10);
                chartCanvas.Children.Add(yLabel);   
            }
            for(int i  = 0; i < pointCount; i++) {

                double xPos = margin + i * xInterval;
                Line xTick = new Line()
                {

                    X1 = xPos,
                    Y1 = margin + usableHeight,
                    X2 = xPos,
                    Y2 = margin + usableHeight + 5,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1

                };
                chartCanvas.Children.Add(xTick);
                TextBlock xLabel = new TextBlock()
                {
                    Text = $"{i + 1}",
                    FontSize = 12
                };
                Canvas.SetLeft(xLabel, xPos - 5);
                Canvas.SetTop(xLabel, margin + usableHeight + 5);
                chartCanvas.Children.Add(xLabel);
                Polyline salesLine = new Polyline()
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2

                };
                for (int j = 0; j < pointCount; j++)
                {
                    double x = margin + j * xInterval;
                    double y = margin + usableHeight - (salesData[j] * yScale);
                    salesLine.Points.Add(new Point(x, y));
                }
                chartCanvas.Children.Add(salesLine);
                TextBlock chartTitle = new TextBlock
                {
                    Text = "Sales Trend",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold
                };
                double titleX = (canvasWidth - chartTitle.ActualWidth) / 2;
                Canvas.SetLeft(chartTitle, margin);
                Canvas.SetTop(chartTitle, 5);
                chartCanvas.Children.Add(chartTitle);
                TextBlock yAxisTitle = new TextBlock
                {
                    Text = "Units Sold",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    RenderTransform = new RotateTransform(-90)

                };
                Canvas.SetLeft(yAxisTitle, 1);
                Canvas.SetTop(yAxisTitle, (canvasHeight / 2) + 10);
                chartCanvas.Children.Add(yAxisTitle);

                TextBlock xAxisTitle = new TextBlock
                {
                    Text = "Days",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold
                };
                Canvas.SetLeft(xAxisTitle,(canvasWidth / 2) - 20);
                Canvas.SetTop(xAxisTitle, canvasHeight - 20);
                chartCanvas.Children.Add(xAxisTitle);
            }
        }

       
    }
}
