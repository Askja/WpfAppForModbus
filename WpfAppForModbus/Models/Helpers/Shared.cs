namespace WpfAppForModBus.Models.Helpers;

public class Shared {
    public static string[] GetAvailablePorts() => System.IO.Ports.SerialPort.GetPortNames();

    public static System.Collections.Generic.IEnumerable<string> GetNames<T>(T[] list)
    where T : WpfAppForModBus.Interfaces.IComboBoxObjects {
        return System.Linq.Enumerable.AsEnumerable(
            source: System.Linq.Enumerable.Select(source: list, selector: o => o.Name));
    }

    public static byte[] HexToByte(string message) {
        message = message.Replace(oldValue: " ", newValue: "");

        byte[] comBuffer = new byte[message.Length / 2];

        for (int i = 0; i < message.Length / 2; i++) {
            comBuffer[i] = System.Convert.ToByte(value: message.Substring(startIndex: i * 2, length: 2), fromBase: 16);
        }

        return comBuffer;
    }

    public static string ByteToHex(byte[] comByte) {
        System.Text.StringBuilder builder = new(capacity: comByte.Length * 3);

        foreach (byte data in comByte) {
            builder.Append(value: System.Convert.ToString(value: data, toBase: 16)
                .PadLeft(totalWidth: 2, paddingChar: '0').PadRight(totalWidth: 3, paddingChar: ' '));
        }

        return builder.ToString().ToUpper();
    }

    public static string GetString(System.Windows.Window window, string key) {
        object? resource = window.FindResource(resourceKey: key);

        if (resource != null) {
            return (string)resource;
        }

        return "";
    }
}