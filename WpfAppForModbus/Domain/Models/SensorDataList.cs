namespace WpfAppForModBus.Domain.Models;

public class SensorDataList : WpfAppForModBus.Domain.Interfaces.ISensorDataList {
    protected ApplicationContext Context;

    public SensorDataList(ApplicationContext context) => Context = context;

    public System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> GetSensorData() {
        return System.Linq.Enumerable.AsEnumerable(
            source: Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(
                source: System.Linq.Queryable.Join(
                    outer: Context.SensorsData, inner: Context.Sensors,
                    outerKeySelector: sensorData => sensorData.SensorId,
                    innerKeySelector: sensor => sensor.SensorId,
                    resultSelector: (sensorData, sensor) => new WpfAppForModBus.Models.Views.SensorView {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        SensorData = sensorData.SensorData,
                        RowId = sensorData.RowId,
                        RowDate = sensorData.RowDate
                    }
                )));
    }

    public System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorView> GetByDate(
        System.DateTime start, System.DateTime end) {
        return System.Linq.Enumerable.AsEnumerable(
            source: Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(
                source: System.Linq.Queryable.Join(
                    outer: System.Linq.Queryable.Where(source: Context.SensorsData,
                        predicate: row => row.RowDate >= start && row.RowDate <= end),
                    inner: Context.Sensors,
                    outerKeySelector: sensorData => sensorData.SensorId,
                    innerKeySelector: sensor => sensor.SensorId,
                    resultSelector: (sensorData, sensor) => new WpfAppForModBus.Models.Views.SensorView {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        SensorData = sensorData.SensorData,
                        RowId = sensorData.RowId,
                        RowDate = sensorData.RowDate
                    }
                )));
    }

    public void AddSensorData(int sensorId, string sensorData) {
        Context.SensorsData.Add(entity: new() {
            RowId = System.Guid.NewGuid(),
            RowDate = System.DateTime.Now,
            SensorData = sensorData,
            SensorId = sensorId
        });
    }

    public void AddSensor(int sensorId, string sensorName) {
        Context.Sensors.Add(entity: new() {
            SensorId = sensorId,
            SensorName = sensorName
        });
    }

    public int GetSensorId(string sensorName) {
        return System.Linq.Queryable.FirstOrDefault(source: System.Linq.Queryable.Select(
            source: System.Linq.Queryable.Where(source: Context.Sensors,
                predicate: sensor => sensor.SensorName.Equals(sensorName)),
            selector: sensor => sensor.SensorId));
    }

    public System.Collections.Generic.IEnumerable<string> GetSensors() {
        return System.Linq.Queryable.Select(source: Context.Sensors, selector: sensor => sensor.SensorName);
    }

    public void DeleteByDate(int sensorId, System.DateTime start, System.DateTime end) {
        System.Linq.IQueryable<WpfAppForModBus.Domain.Entities.SensorsData> list = System.Linq.Queryable.Where(
            source: Context.SensorsData,
            predicate: sensor =>
                sensor.SensorId == sensorId && sensor.RowDate >= start && sensor.RowDate <= end);

        if (System.Linq.Queryable.Any(source: list)) {
            Context.SensorsData.RemoveRange(entities: list);
            Context.SaveChanges();
        }
    }

    public System.Collections.Generic.IEnumerable<WpfAppForModBus.Models.Views.SensorDataGridView> GetSensorData(
        string sensorName) {
        return System.Linq.Enumerable.AsEnumerable(
            source: Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(
                source: System.Linq.Queryable.Join(
                    outer: System.Linq.Queryable.Where(source: Context.Sensors,
                        predicate: sensor => sensor.SensorName.Equals(sensorName)),
                    inner: Context.SensorsData,
                    outerKeySelector: sensor => sensor.SensorId,
                    innerKeySelector: sensorData => sensorData.SensorId,
                    resultSelector: (sensorData, sensor) => new WpfAppForModBus.Models.Views.SensorDataGridView {
                        RowId = sensor.RowId,
                        SensorData = sensor.SensorData,
                        RowDate = sensor.RowDate
                    }
                )));
    }

    public bool SensorExist(int sensorId) {
        return System.Linq.Queryable.Any(
            source: System.Linq.Queryable.Select(source: Context.Sensors, selector: x => x.SensorId == sensorId));
    }

    public void SaveAll() {
        Context.SaveChanges();
    }

    public bool DeleteRow(System.Guid rowId) {
        WpfAppForModBus.Domain.Entities.SensorsData? row =
            System.Linq.Queryable.FirstOrDefault(source: System.Linq.Queryable.Where(source: Context.SensorsData,
                predicate: sensor => sensor.RowId.Equals(rowId)));

        if (row != null) {
            Context.SensorsData.Remove(entity: row);

            return true;
        }

        return false;
    }
}