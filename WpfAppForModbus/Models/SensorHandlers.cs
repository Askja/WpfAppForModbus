using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppForModbus.Domain.Entities;

namespace WpfAppForModbus.Models
{
    public class SensorHandlers {
        private List<SensorData> Sensors { get; set; } = null!;
        private int CurrentPosition { get; set; } = 0;

        public SensorHandlers() {
            Sensors = new List<SensorData>();
        }

        public void AddSensor(SensorData sensor) {
            Sensors.Add(sensor);
        }

        public SensorData Next() {
            CurrentPosition = CurrentPosition < Sensors.Count ? CurrentPosition + 1 : 0;

            return Current();
        }

        public bool Any() {
            return Sensors.Any();
        }

        public SensorData Current() {
            return Sensors[CurrentPosition];
        }

        public double Handle(string Command) {
            return Current().Handler(Command);
        }

        public void FlushAll() {
            Sensors.Clear();
        }
    }
}
