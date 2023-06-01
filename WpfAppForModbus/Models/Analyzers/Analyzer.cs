namespace WpfAppForModBus.Models.Analyzers;

class Analyzer {
    public static WpfAppForModBus.Models.Views.AnalyzerView AnalyzeData(
        System.Collections.Generic.IEnumerable<double> data) {
        if (System.Linq.Enumerable.Any(source: data)) {
            return new() {
                Data = System.Linq.Enumerable.ToList(source: data),
                Max = System.Linq.Enumerable.Max(source: data),
                Min = System.Linq.Enumerable.Min(source: data),
                Mean = CalculateMean(data: System.Linq.Enumerable.ToList(source: data)),
                StandardDeviation = CalculateStandardDeviation(data: System.Linq.Enumerable.ToList(source: data)),
                Mode = CalculateMode(data: System.Linq.Enumerable.ToList(source: data)),
                Median = CalculateMedian(data: System.Linq.Enumerable.ToList(source: data)),
                Dispersion = CalculateVariance(data: System.Linq.Enumerable.ToList(source: data)),
                CoefficientOfVariation =
                    CalculateCoefficientOfVariation(data: System.Linq.Enumerable.ToList(source: data)),
                InterquartileRange = CalculateInterquartileRange(data: System.Linq.Enumerable.ToList(source: data))
            };
        }

        return new();
    }

    private static double CalculateMean(System.Collections.Generic.List<double> data) =>
        data.Count > 0 ? System.Linq.Enumerable.Sum(source: data) / data.Count : 0;

    private static double CalculateStandardDeviation(System.Collections.Generic.List<double> data) {
        double mean = CalculateMean(data: data);
        double sumOfSquaredDifferences =
            System.Linq.Enumerable.Sum(source: data, selector: value => System.Math.Pow(x: value - mean, y: 2));

        double variance = sumOfSquaredDifferences / data.Count;

        return System.Math.Sqrt(d: variance);
    }

    public static double CalculateMedian(System.Collections.Generic.List<double> data) {
        double[] sortedData =
            System.Linq.Enumerable.ToArray(source: System.Linq.Enumerable.OrderBy(source: data, keySelector: x => x));
        int n = sortedData.Length;

        if (n % 2 == 0) {
            return (sortedData[n / 2 - 1] + sortedData[n / 2]) / 2;
        }

        return sortedData[n / 2];
    }

    public static double CalculateMode(System.Collections.Generic.List<double> data) {
        System.Collections.Generic.Dictionary<double, int> frequencyDict = new();

        foreach (double value in data) {
            if (frequencyDict.ContainsKey(key: value)) {
                frequencyDict[key: value]++;
            }
            else {
                frequencyDict[key: value] = 1;
            }
        }

        int maxFrequency = System.Linq.Enumerable.Max(source: frequencyDict.Values);
        double mode = System.Linq.Enumerable
            .FirstOrDefault(source: frequencyDict, predicate: x => x.Value == maxFrequency).Key;

        return mode;
    }

    public static double[] CalculateQuartiles(System.Collections.Generic.List<double> data) {
        double[] sortedData =
            System.Linq.Enumerable.ToArray(source: System.Linq.Enumerable.OrderBy(source: data, keySelector: x => x));
        int n = sortedData.Length;
        double[] quartiles = new double[3];

        quartiles[0] = sortedData[n / 4];
        quartiles[1] = CalculateMedian(data: System.Linq.Enumerable.ToList(source: sortedData));
        quartiles[2] = sortedData[3 * n / 4];

        return quartiles;
    }

    public static double CalculateInterquartileRange(System.Collections.Generic.List<double> data) {
        double[] quartiles = CalculateQuartiles(data: data);

        return quartiles[2] - quartiles[0];
    }

    public static double CalculateCoefficientOfVariation(System.Collections.Generic.List<double> data) {
        double mean = CalculateMean(data: data);
        double standardDeviation = CalculateStandardDeviation(data: data);

        return standardDeviation / mean;
    }

    public static double CalculateVariance(System.Collections.Generic.List<double> data) {
        double mean = CalculateMean(data: data);
        double sumOfSquaredDifferences = 0;

        foreach (double value in data) {
            double difference = value - mean;
            sumOfSquaredDifferences += difference * difference;
        }

        return sumOfSquaredDifferences / data.Count;
    }
}