using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Senior_Project
{
    public static class SalesPrediction
    {
        public static double PredictTotalSales(int daysAhead)
        {
            var sales = Sales.LoadSales();
            if(!sales.Any()) { return 0; }
            
            var grouped = sales
                .GroupBy(s=> s.SaleDate.Date)
                .Select(g=> new {Date = g.Key, Quantity  = g.Sum(s=> s.Quantity) })
                .OrderBy(g=>g.Date)
                .ToList();
            var startDate = grouped.First().Date;
            var x = grouped.Select((g,i) => (double)(g.Date - startDate).TotalDays).ToArray();
            var y = grouped.Select(g => (double)g.Quantity).ToArray();
            
            LinearRegression(x, y, out double slope, out double intercept);
            double lastX = x.Last();
            double futureX = lastX + daysAhead;
            return slope * futureX + intercept; 
        }
        public static Dictionary<string, double> PredictAllProducts(List<Sales> salesData, int daysAhead)
        {
            var predictions = new Dictionary<string, double>();
            var products = salesData.Select(s => s.ProductName).Distinct();
            foreach ( var product in products)
            {
                var filtered = salesData
                    .Where(s=> s.ProductName.Equals(product, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(s=> s.SaleDate.Date)
                    .Select(g=> new {Date = g.Key, Quantity = g.Sum(s=>s.Quantity) })
                    .OrderBy(g=>g.Date)
                    .ToList();
                if (filtered.Count < 2) continue;
                var start = filtered.First().Date;
                var x = filtered.Select(g=>(double)(g.Date - start).TotalDays).ToArray();
                var y = filtered.Select(g=>(double)g.Quantity).ToArray();
                LinearRegression(x,y,out double slope, out double intercept);
                double prediction = slope * (x.Last() + daysAhead) + intercept;
                predictions[product] = Math.Max(prediction, 0);
            }
            return predictions;
        }
        public static double PredictSalesUsingData(List<Sales> salesData, string productName, int daysAhead)
        {
            var productSales = salesData
                .Where(s => s.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new { Date = g.Key, Quantity = g.Sum(s => s.Quantity) })
                .OrderBy(g => g.Date)
                .ToList();

            if (productSales.Count < 2) return 0;

            var startDate = productSales.First().Date;
            var x = productSales.Select(g => (double)(g.Date - startDate).TotalDays).ToArray();
            var y = productSales.Select(g => (double)g.Quantity).ToArray();

            LinearRegression(x, y, out double slope, out double intercept);
            double futureX = x.Last() + daysAhead;
            return slope * futureX + intercept;
        }

        public static void LinearRegression(double[]x, double[] y, out double slope, out double intercept)
        {
            double n = x.Length;
            double sumX = x.Sum();
            double sumY = y.Sum();
            double sumXY = x.Zip(y,(a, b)=> a*b).Sum();
            double sumX2 = x.Select(v=> v*v).Sum();

            slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            intercept = (sumY - slope * sumX) / n;
        }
       
       
    }
}

