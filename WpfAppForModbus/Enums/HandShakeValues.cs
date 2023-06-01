namespace WpfAppForModBus.Enums;

public class HandshakeValues : WpfAppForModBus.Interfaces.IComboBoxObjects {
    public System.IO.Ports.Handshake Type { get; set; }

    public required string Name { get; set; }
}