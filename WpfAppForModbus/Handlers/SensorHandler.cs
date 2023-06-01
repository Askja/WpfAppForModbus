namespace WpfAppForModBus.Handlers;

public static class SensorHandler {
    public static string[] SplitCommand(string command) => command.Split(separator: " ");

    public static double CountTemperature(string result) {
        string[] registers = SplitCommand(command: result);

        int decimalNumber = System.Convert.ToInt32(value: registers[3] + registers[4], fromBase: 16);

        return decimalNumber > 0 ? decimalNumber / 205.0 : 0;
    }

    public static double CountBar(string result) {
        string[] registers = SplitCommand(command: result);

        int decimalNumber = System.Convert.ToInt32(value: registers[3] + registers[4], fromBase: 16);

        return decimalNumber > 0 ? decimalNumber / 205.0 : 0;
    }

    public static double CountWater(string result) {
        string[] registers = SplitCommand(command: result);

        int decimalNumber = System.Convert.ToInt32(value: registers[3] + registers[4], fromBase: 16);

        return decimalNumber > 0 ? decimalNumber / 205.0 : 0;
    }

    public static double CountVoltage(string result) {
        string[] registers = SplitCommand(command: result);

        int decimalNumber = System.Convert.ToInt32(value: registers[3] + registers[4], fromBase: 16);

        return decimalNumber > 0 ? decimalNumber / 205.0 : 0;
    }
}