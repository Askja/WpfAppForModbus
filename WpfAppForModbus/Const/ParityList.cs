using System.IO.Ports;
using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Const {
    public static class ParityList {
        public static ParityValues[] Parities { get; } = new ParityValues[] {
            new ParityValues { Name = "None", Type = Parity.None },
            new ParityValues { Name = "Odd", Type = Parity.Odd },
            new ParityValues { Name = "Even", Type = Parity.Even }
        };
    }
}
