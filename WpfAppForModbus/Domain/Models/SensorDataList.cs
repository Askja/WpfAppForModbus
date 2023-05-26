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

        public bool SensorExist(int SensorId) {
            return context.Sensors.Select(x => x.SensorId == SensorId).Any();
        }

        public void SaveAll() {
            context.SaveChanges();
        }
    }
}
