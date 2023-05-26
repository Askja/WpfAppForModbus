using System;
using System.IO.Ports;
using System.Text;
using WpfAppForModbus.Models.Helpers;
using WpfAppForModbus.Models.Views;

namespace WpfAppForModbus.Models.Core {
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
            if (Options != null && Options.IsValid()) {
                Port = new SerialPort(
                    Options.SelectedPort ?? string.Empty,
                    Options.SelectedBaudRate,
                    Options?.SelectedParity?.Type ?? new(),
                    Options?.SelectedDataBits ?? new(),
                    Options?.SelectedStopBits?.Type ?? new()
                ) {
                    ReadTimeout = DefaultTimeout,
                    WriteTimeout = DefaultTimeout,
                    Handshake = Options?.SelectedHandshake?.Type ?? new()
                };

                if (Options != null) {
                    Port.DataReceived += Options.Handler;
                }

                Port.Open();

                return IsOpened();
            } else {
                throw new ArgumentNullException();
            }
        }

        public bool Open(ComPortOptions options) {
            Options = options;

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

                    return Shared.ByteToHex(buffer);
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

        public void Write(string buffer) {
            byte[] bytes = Shared.HexToByte(buffer);

            Write(bytes, 0, bytes.Length);
        }
    }
}