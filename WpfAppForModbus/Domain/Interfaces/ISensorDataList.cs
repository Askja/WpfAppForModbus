using System.Collections.Generic;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Domain.Interfaces {
    public interface ISensorDataList {
        void AddSensor(int SensorId, string SensorName);
        void AddSensorData(int SensorId, string SensorData);
        IEnumerable<SensorView> GetSensorData();
        int SaveAll();
        bool SensorExist(int SensorId);
    }
}
