using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Models {
    public class ComPortOptions {
        public ParityValues? SelectedParity { get; set; }
        public StopBitsValues? SelectedStopBits { get; set; }
        public HandshakeValues? SelectedHandshake { get; set; }
        public int? SelectedDataBits { get; set; }
        public int? SelectedBaudRate { get; set; }
        public string? SelectedPort { get; set; }

        public bool IsValid() => SelectedParity != null && SelectedStopBits != null && SelectedHandshake != null && SelectedDataBits != null && SelectedBaudRate != null && SelectedPort != null;
    }
}
