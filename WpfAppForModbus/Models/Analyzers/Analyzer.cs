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
                    StandardDeviation = CalculateStandardDeviation(data.ToList()),
                    Mode = CalculateMode(data.ToList()),
                    Median = CalculateMedian(data.ToList()),
                    Dispersion = CalculateVariance(data.ToList()),
                    CoefficientOfVariation = CalculateCoefficientOfVariation(data.ToList()),
                    InterquartileRange = CalculateInterquartileRange(data.ToList())
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

        public static double CalculateMedian(List<double> data) {
            double[] sortedData = data.OrderBy(x => x).ToArray();
            int n = sortedData.Length;

            if (n % 2 == 0) {
                return (sortedData[(n / 2) - 1] + sortedData[n / 2]) / 2;
            } else {
                return sortedData[n / 2];
            }
        }

        public static double CalculateMode(List<double> data) {
            Dictionary<double, int> frequencyDict = new();

            foreach (double value in data) {
                if (frequencyDict.ContainsKey(value)) {
                    frequencyDict[value]++;
                } else {
                    frequencyDict[value] = 1;
                }
            }

            int maxFrequency = frequencyDict.Values.Max();
            double mode = frequencyDict.FirstOrDefault(x => x.Value == maxFrequency).Key;

            return mode;
        }

        public static double[] CalculateQuartiles(List<double> data) {
            double[] sortedData = data.OrderBy(x => x).ToArray();
            int n = sortedData.Length;
            double[] quartiles = new double[3];

            quartiles[0] = sortedData[n / 4];
            quartiles[1] = CalculateMedian(sortedData.ToList());
            quartiles[2] = sortedData[(3 * n) / 4];

            return quartiles;
        }

        public static double CalculateInterquartileRange(List<double> data) {
            double[] quartiles = CalculateQuartiles(data);

            return quartiles[2] - quartiles[0];
        }

        public static double CalculateCoefficientOfVariation(List<double> data) {
            double mean = CalculateMean(data);
            double standardDeviation = CalculateStandardDeviation(data);

            return standardDeviation / mean;
        }
        public static double CalculateVariance(List<double> data) {
            double mean = CalculateMean(data);
            double sumOfSquaredDifferences = 0;

            foreach (double value in data) {
                double difference = value - mean;
                sumOfSquaredDifferences += difference * difference;
            }

            return sumOfSquaredDifferences / data.Count;
        }
    }
}
