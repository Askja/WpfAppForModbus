using System;

namespace WpfAppForModbus.Models.Views {
    public class SensorDataGridView {
        public Guid RowId {
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
