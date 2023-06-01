namespace WpfAppForModBus.Models.Views;

public class SensorView {
    public System.Guid RowId { get; set; }

    public int SensorId { get; set; }

    public required string SensorName { get; set; }

    public required string SensorData { get; set; }

    public System.DateTime RowDate { get; set; }
}