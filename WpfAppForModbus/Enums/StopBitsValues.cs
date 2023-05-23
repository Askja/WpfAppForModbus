using System.IO.Ports;
using WpfAppForModbus.Interfaces;

namespace WpfAppForModbus.Enums {
    public class StopBitsValues : IComboBoxObjects {
        public required string Name {
            get; set;
        }
        public StopBits Type {
            get; set;
        }

    }
}
