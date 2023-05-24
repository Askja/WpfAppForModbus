using System.IO.Ports;
using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Const {
    public static class StopBitsList {
        public static StopBitsValues[] StopBitsTypes {
            get;
        } = new StopBitsValues[] {
            new StopBitsValues { Name = "One", Type = StopBits.One },
            new StopBitsValues { Name = "Two", Type = StopBits.Two },
            new StopBitsValues { Name = "None", Type = StopBits.None }
        };
    }
}
