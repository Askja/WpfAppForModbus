namespace WpfAppForModBus.Models.Views;

public class SensorDataGridView {
    public System.Guid RowId { get; set; }

    public required string SensorData { get; set; }

    public System.DateTime RowDate { get; set; }
}