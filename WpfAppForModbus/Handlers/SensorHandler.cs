using System;

namespace WpfAppForModbus.Handlers {
    public static class SensorHandler {
        public static string[] SplitCommand(string Command) {
            return Command.Split(" ");
        }

        public static double CountTemperature(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[3] + Registers[4], 16);

            return DecimalNumber > 0 ? DecimalNumber / 205.0 : 0;
        }

        public static double CountBar(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[3] + Registers[4], 16);

            return DecimalNumber > 0 ? DecimalNumber / 205.0 : 0;
        }

        public static double CountWater(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[3] + Registers[4], 16);

            return DecimalNumber > 0 ? DecimalNumber / 205.0 : 0;
        }

        public static double CountVoltage(string Result) {
            string[] Registers = SplitCommand(Result);

            int DecimalNumber = Convert.ToInt32(Registers[3] + Registers[4], 16);

            return DecimalNumber > 0 ? DecimalNumber / 205.0 : 0;
        }
    }
}
