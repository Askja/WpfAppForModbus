using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using WpfAppForModbus.Interfaces;

namespace WpfAppForModbus.Models.Helpers {
    public class Shared {
        public static string[] GetAvailablePorts() {
            return SerialPort.GetPortNames();
        }

        public static IEnumerable<string> GetNames<T>(T[] List) where T : IComboBoxObjects {
            return List.Select(o => o.Name).AsEnumerable();
        }

        public static byte[] HexToByte(string message) {
            message = message.Replace(" ", "");

            byte[] comBuffer = new byte[message.Length / 2];

            for (int i = 0; i < message.Length / 2; i++) {
                comBuffer[i] = Convert.ToByte(message.Substring(i * 2, 2), 16);
            }

            return comBuffer;
        }
        public static string ByteToHex(byte[] comByte) {
            StringBuilder builder = new(comByte.Length * 3);

            foreach (byte data in comByte) {
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return builder.ToString().ToUpper();
        }

        public static string GetString(Window window, string Key) {
            object Resource = window.FindResource(Key);

            if (Resource != null) {
                return (string)Resource;
            }

            return "";
        }
    }
}