namespace WpfAppForModBus.Domain.Interfaces;

public interface ISensorDataList {
    void AddSensor(int sensorId, string sensorName);
    void AddSensorData(int sensorId, string sensorData);
    void DeleteByDate(int sensorId, System.DateTime start, System.DateTime end);

    System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> GetByDate(System.DateTime start,
        System.DateTime end);

    System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> GetSensorData();

    System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorDataGridView> GetSensorData(
        string sensorName);

    int GetSensorId(string sensorName);
    System.Collections.Generic.IEnumerable<string> GetSensors();
    void SaveAll();
    bool SensorExist(int sensorId);
}