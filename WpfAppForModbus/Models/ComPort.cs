using System;
using System.IO.Ports;
using System.Text;
using System.Windows;

namespace WpfAppForModbus.Models {
    public class ComPort {

        protected SerialPort? Port { get; set; } = null;

        private ComPortOptions Options { get; set; } = null!;

        protected int DefaultTimeout = 500;

        public ComPort() { }

        public ComPort(ComPortOptions Options) {
            this.Options = Options;
        }

        public bool IsOpened() {
            return Port != null && Port.IsOpen;
        }

        public bool IsClosed() {
            return !IsOpened();
        }

        public bool Open() {
            try {
                if (Options != null && Options.IsValid()) {
                    Port = new SerialPort(
                        Options.SelectedPort,
                        (int)Options.SelectedBaudRate,
                        (Parity)(Options.SelectedParity.Type),
                        (int)Options.SelectedDataBits,
                        (StopBits)(Options.SelectedStopBits.Type)
                    ) {
                        ReadTimeout = DefaultTimeout,
                        WriteTimeout = DefaultTimeout,
                        Handshake = (Handshake)(Options.SelectedHandshake.Type)
                    };

                    Port.Open();

                    return IsOpened();
                } else {
                    throw new ArgumentNullException();
                }
            } catch (Exception exception) {
                MessageBox.Show(exception.Message);
            }

            return false;
        }

        public bool Open(ComPortOptions Options) {
            this.Options = Options;

            return Open();
        }

        public bool Close() {
            if (IsOpened()) {
                try {
                    Port?.Close();

                    return true;
                } catch (Exception) { }

                return false;
            }

            return true;
        }

        public string Read() {
            if (IsOpened()) {
                try {
                    byte[] buffer = new byte[Port?.BytesToRead ?? 0];
                    Port?.Read(buffer, 0, buffer.Length);
                    return Helpers.ByteToHex(buffer);
                } catch (TimeoutException) { }


            }

            return "";
        }

        public void Write(byte[] buffer, int offset, int count) {
            if (IsOpened()) {
                try {
                    Port?.Write(buffer, offset, count);
                } catch (Exception) { }
            }
        }

        public void Write(char[] buffer, int offset, int count) {
            Write(Encoding.GetEncoding("UTF-8").GetBytes(buffer), offset, count);
        }
        public void Write(string buffer) {
            byte[] bytes = Helpers.HexToByte(buffer);

            Write(bytes, 0, bytes.Length);
        }
    }
}