using System.Collections.Generic;

namespace WpfAppForModbus.Models.Views {
    public class AnalyzerView {
        public List<double> Data { get; set; } = null!;
        public double Max { get; set; } = -1;
        public double Min { get; set; } = -1;
        public double Mean { get; set; } = -1;
        public double StandardDeviation { get; set; } = -1;
    }
}
