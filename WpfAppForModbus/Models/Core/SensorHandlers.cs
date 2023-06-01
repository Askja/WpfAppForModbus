namespace WpfAppForModBus.Models.Core;

public class SensorHandlers {
    public SensorHandlers() => Sensors = new();

    private System.Collections.Generic.List<WpfAppForModBus.Models.Views.SensorData> Sensors { get; } = null!;
    private int CurrentPosition { get; set; }

    public void AddSensor(WpfAppForModBus.Models.Views.SensorData sensor) {
        Sensors.Add(item: sensor);
    }

    public WpfAppForModBus.Models.Views.SensorData Next() {
        CurrentPosition = CurrentPosition < Sensors.Count - 1 ? CurrentPosition + 1 : 0;

        return Current();
    }

    public bool Any() => System.Linq.Enumerable.Any(source: Sensors);

    public WpfAppForModBus.Models.Views.SensorData Current() => Sensors[index: CurrentPosition];

    public int GetCurrentId() => Current().Id;

    public string GetCurrentName() => Current().Name;

    public string GetCurrentCommand() => Current().Command;

    public string GetCurrentRecommendations() => Current().Recommendations;

    public double Handle(string command) => Current().Handler(arg: command);

    public void FlushAll() {
        Sensors.Clear();
    }
}