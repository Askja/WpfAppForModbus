using System.Collections.Generic;
using System.Linq;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Models.Core {
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
            CurrentPosition = CurrentPosition < Sensors.Count - 1 ? CurrentPosition + 1 : 0;

            return Current();
        }

        public bool Any() {
            return Sensors.Any();
        }

        public SensorData Current() {
            return Sensors[CurrentPosition];
        }

        public int GetCurrentId() {
            return Current().Id;
        }

        public string GetCurrentName() {
            return Current().Name;
        }

        public string GetCurrentCommand() {
            return Current().Command;
        }

        public string GetCurrentRecommendations() {
            return Current().Recommendations;
        }

        public double Handle(string Command) {
            return Current().Handler(Command);
        }

        public void FlushAll() {
            Sensors.Clear();
        }
    }
}
