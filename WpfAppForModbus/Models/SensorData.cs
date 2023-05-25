using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppForModbus.Domain.Entities;

namespace WpfAppForModbus.Models
{
    public class SensorData {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Command { get; set; } = null!;
        public Func<string, double> Handler { get; set; } = null!;
    }
}
