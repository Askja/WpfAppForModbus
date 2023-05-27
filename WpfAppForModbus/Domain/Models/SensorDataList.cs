using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfAppForModbus.Domain.Entities;
using WpfAppForModbus.Domain.Interfaces;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Domain.Models {
    public class SensorDataList : ISensorDataList {
        protected ApplicationContext context;

        public SensorDataList(ApplicationContext context) {
            this.context = context;
        }

        public IEnumerable<SensorView> GetSensorData() {
            return context.SensorsData
                .Join(
                    context.Sensors,
                    sensorData => sensorData.SensorId,
                    sensor => sensor.SensorId,
                    (sensorData, sensor) => new SensorView() {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        SensorData = sensorData.SensorData,
                        RowId = sensorData.RowId,
                        RowDate = sensorData.RowDate
                    }
                ).AsNoTracking().AsEnumerable();
        }

        public IEnumerable<SensorView> GetByDate(DateTime Start, DateTime End) {
            return context.SensorsData
                .Where(Row => Row.RowDate >= Start && Row.RowDate <= End)
                .Join(
                    context.Sensors,
                    sensorData => sensorData.SensorId,
                    sensor => sensor.SensorId,
                    (sensorData, sensor) => new SensorView() {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        SensorData = sensorData.SensorData,
                        RowId = sensorData.RowId,
                        RowDate = sensorData.RowDate
                    }
                ).AsNoTracking().AsEnumerable();
        }

        public void AddSensorData(int SensorId, string SensorData) {
            context.SensorsData.Add(new SensorsData() {
                RowId = Guid.NewGuid(),
                RowDate = DateTime.Now,
                SensorData = SensorData,
                SensorId = SensorId
            });
        }

        public void AddSensor(int SensorId, string SensorName) {
            context.Sensors.Add(new Sensor() {
                SensorId = SensorId,
                SensorName = SensorName
            });
        }

        public int GetSensorId(string SensorName) {
            return context.Sensors.Where(Sensor => Sensor.SensorName.Equals(SensorName)).Select(Sensor => Sensor.SensorId).FirstOrDefault();
        }

        public IEnumerable<string> GetSensors() {
            return context.Sensors.Select(Sensor => Sensor.SensorName);
        }

        public bool DeleteRow(Guid RowId) {
            var Row = context.SensorsData.Where(Sensor => Sensor.RowId.Equals(RowId)).FirstOrDefault();

            if (Row != null) {
                context.SensorsData.Remove(Row);

                return true;
            }

            return false;
        }

        public void DeleteByDate(int SensorId, DateTime Start, DateTime End) {
            var List = context.SensorsData.Where(Sensor => Sensor.SensorId == SensorId && Sensor.RowDate >= Start && Sensor.RowDate <= End);

            if (List.Any()) {
                context.SensorsData.RemoveRange(List);
                context.SaveChanges();
            }
        }

        public IEnumerable<SensorDataGridView> GetSensorData(string SensorName) {
            return context.Sensors
                .Where(Sensor => Sensor.SensorName.Equals(SensorName))
                .Join(
                    context.SensorsData,
                    sensor => sensor.SensorId,
                    sensorData => sensorData.SensorId,
                    (sensorData, sensor) => new SensorDataGridView() {
                        RowId = sensor.RowId,
                        SensorData = sensor.SensorData,
                        RowDate = sensor.RowDate
                    }
                ).AsNoTracking().AsEnumerable();
        }

        public bool SensorExist(int SensorId) {
            return context.Sensors.Select(x => x.SensorId == SensorId).Any();
        }

        public void SaveAll() {
            context.SaveChanges();
        }
    }
}
