using System;
using System.Collections.Generic;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Domain.Interfaces {
    public interface ISensorDataList {
        void AddSensor(int SensorId, string SensorName);
        void AddSensorData(int SensorId, string SensorData);
        void DeleteByDate(int SensorId, DateTime Start, DateTime End);
        IEnumerable<SensorView> GetByDate(DateTime Start, DateTime End);
        IEnumerable<SensorView> GetSensorData();
        IEnumerable<SensorDataGridView> GetSensorData(string SensorName);
        int GetSensorId(string SensorName);
        IEnumerable<string> GetSensors();
        void SaveAll();
        bool SensorExist(int SensorId);
    }
}
