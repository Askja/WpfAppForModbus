namespace WpfAppForModBus.Models.Views;

public class SensorData {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Command { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
    public double Max { get; set; } = double.MaxValue;
    public System.Func<string, double> Handler { get; set; } = null!;
}