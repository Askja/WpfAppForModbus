using System.Collections.Generic;
using WpfAppForModbus.Models;

namespace WpfAppForModbus.Domain.Interfaces {
    public interface ISensorDataList {
        IEnumerable<SensorView> GetSensorData();
    }
}
