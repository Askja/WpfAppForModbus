using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WpfAppForModbus.Domain.Interfaces;
using WpfAppForModbus.Models;

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
    }
}
