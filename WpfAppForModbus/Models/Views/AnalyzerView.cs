using System.Collections.Generic;

namespace WpfAppForModbus.Models.Views {
    public class AnalyzerView {
        public List<double> Data { get; set; } = null!;
        public double Max { get; set; } = -1;
        public double Min { get; set; } = -1;
        public double Mean { get; set; } = -1;
        public double StandardDeviation { get; set; } = -1;
        public double Mode { get; set; } = -1;
        public double Median { get; set; } = -1;
        public double Dispersion { get; set; } = -1;
        public double InterquartileRange { get; set; } = -1;
        public double CoefficientOfVariation { get; set; } = -1;
    }
}
