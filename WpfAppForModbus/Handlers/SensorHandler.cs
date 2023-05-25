using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

            return 4095 / Convert.ToInt32(Registers[4], 16);
        }

        public static double CountBar(string Result) {
            string[] Registers = SplitCommand(Result);

            return 4095 / Convert.ToInt32(Registers[4], 16);
        }

        public static double CountWater(string Result) {
            string[] Registers = SplitCommand(Result);

            return 4095 / Convert.ToInt32(Registers[4], 16);
        }

        public static double CountVoltage(string Result) {
            string[] Registers = SplitCommand(Result);

            return 4095 / Convert.ToInt32(Registers[4], 16);
        }
    }
}
