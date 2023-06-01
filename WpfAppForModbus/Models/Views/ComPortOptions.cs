namespace WpfAppForModBus.Models.Views;

public class ComPortOptions {
    public WpfAppForModBus.Enums.ParityValues? SelectedParity { get; set; } = null;
    public WpfAppForModBus.Enums.StopBitsValues? SelectedStopBits { get; set; } = null;
    public WpfAppForModBus.Enums.HandshakeValues? SelectedHandshake { get; set; } = null;
    public System.IO.Ports.SerialDataReceivedEventHandler Handler { get; set; } = null!;
    public int SelectedDataBits { get; set; } = -1;
    public int SelectedBaudRate { get; set; } = -1;
    public string? SelectedPort { get; set; } = null;

    public bool IsValid() =>
        SelectedParity != null &&
        SelectedStopBits != null &&
        SelectedHandshake != null &&
        SelectedDataBits > -1 &&
        SelectedBaudRate > -1 &&
        !string.IsNullOrEmpty(value: SelectedPort) &&
        Handler != null;
}