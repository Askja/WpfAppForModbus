namespace WpfAppForModBus.Enums;

public class ParityValues : WpfAppForModBus.Interfaces.IComboBoxObjects {
    public System.IO.Ports.Parity Type { get; set; }

    public required string Name { get; set; }
}