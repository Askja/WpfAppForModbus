namespace WpfAppForModBus.Const;

public static class HandshakeList {
    public static WpfAppForModBus.Enums.HandshakeValues[] Handshakes { get; } = {
        new() { Name = "RequestToSend", Type = System.IO.Ports.Handshake.RequestToSend },
        new() { Name = "RequestToSendXOnXOff", Type = System.IO.Ports.Handshake.RequestToSendXOnXOff },
        new() { Name = "XOnXOff", Type = System.IO.Ports.Handshake.XOnXOff }
    };
}