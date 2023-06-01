namespace WpfAppForModBus.Enums;

public class StopBitsValues : WpfAppForModBus.Interfaces.IComboBoxObjects {
    public System.IO.Ports.StopBits Type { get; set; }

    public required string Name { get; set; }
}