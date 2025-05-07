using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senior_Project
{
    public class MovingAveragePrediction
    {
        private readonly List<int> salesData;
        public MovingAveragePrediction(List<int> salesData)
        {
            this.salesData = salesData ?? throw new ArgumentNullException(nameof(salesData), "Sales data cannot be null");
            if (salesData.Count == 0)
            {
                throw new ArgumentNullException("Sales data cannot be empty. ");

            }
        }
        public List<double> PredictDays(int windowSize, int daysAhead)
        {
            if (salesData == null || salesData.Count == 0)
            {
                throw new ArgumentException("Sales data cannot be empty.");
            }

            List<double> predictions = new List<double>();

            for (int i = 0; i < daysAhead; i++)
            {
                if (salesData.Count < windowSize)
                    throw new ArgumentException("Not enough data.");

                double average = salesData.Skip(salesData.Count - windowSize).Take(windowSize).Average();
                predictions.Add(average);
            }

            return predictions;
        }
    }
}
