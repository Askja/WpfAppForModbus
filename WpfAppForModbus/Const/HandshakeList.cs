using System.IO.Ports;
using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Const {
    public static class HandshakeList {
        public static HandshakeValues[] Handshakes { get; } = new HandshakeValues[] {
            new HandshakeValues { Name = "RequestToSend", Type = Handshake.RequestToSend },
            new HandshakeValues { Name = "RequestToSendXOnXOff", Type = Handshake.RequestToSendXOnXOff },
            new HandshakeValues { Name = "XOnXOff", Type = Handshake.XOnXOff }
        };
    }
}
