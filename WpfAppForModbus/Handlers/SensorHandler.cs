using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfAppForModbus.Handlers
{
    public static class SensorHandler {
        public static string[] SplitCommand(string Command) {
            return Command.Split(" ");
        }

        public static double CountTemperature(string Result) {
            string[] Registers = SplitCommand(Result);

            byte Response = Convert.ToByte("4" + Registers[4]);

            return 4095 / int.Parse(Convert.ToString(Response, 10));
        }

        public static double CountBar(string Result) {
            string[] Registers = SplitCommand(Result);

            byte Response = Convert.ToByte("4" + Registers[4]);

            return 4095 / int.Parse(Convert.ToString(Response, 10));
        }

        public static double CountWater(string Result) {
            string[] Registers = SplitCommand(Result);

            byte Response = Convert.ToByte("4" + Registers[4]);

            return 4095 / int.Parse(Convert.ToString(Response, 10));
        }

        public static double CountVoltage(string Result) {
            string[] Registers = SplitCommand(Result);

            byte Response = Convert.ToByte("4" + Registers[4]);

            return 4095 / int.Parse(Convert.ToString(Response, 10));
        }
    }
}
