using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Models {
    public class ComPortOptions {
        public ParityValues SelectedParity { get; set; } = null!;
        public StopBitsValues SelectedStopBits { get; set; } = null!;
        public HandshakeValues SelectedHandshake { get; set; } = null!;
        public int SelectedDataBits { get; set; } = -1;
        public int SelectedBaudRate { get; set; } = -1;
        public string SelectedPort { get; set; } = string.Empty;

        public bool IsValid() => SelectedParity != null && SelectedStopBits != null && SelectedHandshake != null && SelectedDataBits > -1 && SelectedBaudRate > -1 && !string.IsNullOrEmpty(SelectedPort);
    }
}
