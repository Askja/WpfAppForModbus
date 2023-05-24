using System.Collections.Generic;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Domain.Interfaces {
    public interface ISensorDataList {
        IEnumerable<SensorView> GetSensorData();
    }
}
