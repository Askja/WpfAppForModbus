using WpfAppForModbus.Enums;

namespace WpfAppForModbus.Const {
    public static class StopBitsList {
        public static StopBitsValues[] StopBits {
            get;
        } = new StopBitsValues[] {
            new StopBitsValues { Name = "One", Type = System.IO.Ports.StopBits.One },
            new StopBitsValues { Name = "Two", Type = System.IO.Ports.StopBits.Two },
            new StopBitsValues { Name = "None", Type = System.IO.Ports.StopBits.None }
        };
    }
}
