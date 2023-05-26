using System;
using System.Collections.Generic;
using System.Linq;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Models.Analyzers {
    class Analyzer {
        public static AnalyzerView AnalyzeData(IEnumerable<double> data) {
            if (data.Any()) {
                return new() {
                    Data = data.ToList(),
                    Max = data.Max(),
                    Min = data.Min(),
                    Mean = CalculateMean(data.ToList()),
                    StandardDeviation = CalculateStandardDeviation(data.ToList())
                };
            }

            return new();
        }

        private static double CalculateMean(List<double> data) {
            return data.Count > 0 ? data.Sum() / data.Count : 0;
        }

        private static double CalculateStandardDeviation(List<double> data) {
            double mean = CalculateMean(data);
            double sumOfSquaredDifferences = data.Sum(value => Math.Pow(value - mean, 2));

            double variance = sumOfSquaredDifferences / data.Count;
            return Math.Sqrt(variance);
        }
    }
}
