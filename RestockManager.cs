using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senior_Project
{
    public class RestockManager
    {
        private static readonly PriorityQueue<RestockItem> restockQueue = new();
        public class UrgentProduct
        {
            public string ProductName { get; set; }
            public int RemainingQuantity { get; set; }
        } 
        public static void MonitorStockAfterSale(string productName)
        {
            int quantity = Products.GetProductQuantity(productName);
            if (quantity <= 3)
            {
                EnqueueProduct(productName, quantity);
            }
        }

        private static void EnqueueProduct(string productName, int quantity)
        {
            var item = new RestockItem(productName, quantity);
            restockQueue.Enqueue(item);
            LogActivity.WriteLog($"Added {productName} to restock queue with {quantity} units left.");
        }
        public static void RebuildUrgentQ()
        {
            while (restockQueue.Count > 0)
            {
                restockQueue.Dequeue();
            }

            var products = Products.LoadProducts();
            foreach (var product in products)
            {
                if (product.Quantity <= 3)
                {
                    EnqueueProduct(product.Name, product.Quantity);
                }
            }
        }
        public static List<UrgentProduct> GetUrgentProducts()
        {
            var urgentProducts = Products.LoadProducts()
                .Where(p => p.Quantity <= 3)
                .Select(p => new UrgentProduct { ProductName = p.Name, RemainingQuantity = p.Quantity })
                .ToList();

            return urgentProducts;
        }
        private class RestockItem : IComparable<RestockItem>
        {
            public string ProductName { get; }
            public int RemainingQuantity { get; }

            public RestockItem(string productName, int remainingQuantity)
            {
                ProductName = productName;
                RemainingQuantity = remainingQuantity;
            }

            public int CompareTo(RestockItem other)
            {
                if (other == null) return -1;
                return RemainingQuantity.CompareTo(other.RemainingQuantity);
            }
        }
    }
} 
