using System.IO.Ports;
using WpfAppForModbus.Interfaces;

namespace WpfAppForModbus.Enums {
    public class ParityValues : IComboBoxObjects {
        public required string Name {
            get; set;
        }
        public Parity Type {
            get; set;
        }
    }
}
