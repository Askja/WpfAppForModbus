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

            int DecimalNumber = Convert.ToInt32(Registers[4], 16);

            return DecimalNumber > 0 ? 4095.0 / (double)DecimalNumber * 5.0 : 0;
        }

        public static double CountBar(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[4], 16);

            return DecimalNumber > 0 ? 4095.0 / (double)DecimalNumber * 5.0 : 0;
        }

        public static double CountWater(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[4], 16);

            return DecimalNumber > 0 ? 4095.0 / (double)DecimalNumber * 5.0 : 0;
        }

        public static double CountVoltage(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[4], 16);

            return DecimalNumber > 0 ? 4095.0 / (double)DecimalNumber * 5.0 : 0;
        }
    }
}
