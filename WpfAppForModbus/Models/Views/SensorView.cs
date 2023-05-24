using System;

namespace WpfAppForModbus.Models.Views {
    public class SensorView {
        public int RowId {
            get; set;
        }

        public int SensorId {
            get; set;
        }

        public required string SensorName {
            get; set;
        }

        public required string SensorData {
            get; set;
        }

        public DateTime RowDate {
            get; set;
        }
    }
}
