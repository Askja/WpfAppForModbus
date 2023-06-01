namespace WpfAppForModBus.Models.Core;

public class ComPort {
    protected int DefaultTimeout = 500;

    public ComPort() { }

    public ComPort(WpfAppForModBus.Models.Views.ComPortOptions options) => Options = options;

    protected System.IO.Ports.SerialPort? Port { get; set; }

    private WpfAppForModBus.Models.Views.ComPortOptions Options { get; set; } = null!;

    public bool IsOpened() => Port != null && Port.IsOpen;

    public bool IsClosed() => !IsOpened();

    public bool Open() {
        if (Options != null && Options.IsValid()) {
            Port = new(
                portName: Options.SelectedPort ?? string.Empty,
                baudRate: Options.SelectedBaudRate,
                parity: Options?.SelectedParity?.Type ?? new System.IO.Ports.Parity(),
                dataBits: Options?.SelectedDataBits ?? new int(),
                stopBits: Options?.SelectedStopBits?.Type ?? new System.IO.Ports.StopBits()
            ) {
                ReadTimeout = DefaultTimeout,
                WriteTimeout = DefaultTimeout,
                Handshake = Options?.SelectedHandshake?.Type ?? new System.IO.Ports.Handshake()
            };

            if (Options != null) {
                Port.DataReceived += Options.Handler;
            }

            Port.Open();

            return IsOpened();
        }

        throw new System.ArgumentNullException();
    }

    public bool Open(WpfAppForModBus.Models.Views.ComPortOptions options) {
        Options = options;

        return Open();
    }

    public bool Close() {
        if (IsOpened()) {
            try {
                Port?.Close();

                return true;
            }
            catch (System.Exception) { }

            return false;
        }

        return true;
    }

    public string Read() {
        if (IsOpened()) {
            try {
                byte[] buffer = new byte[Port?.BytesToRead ?? 0];

                Port?.Read(buffer: buffer, offset: 0, count: buffer.Length);

                return WpfAppForModBus.Models.Helpers.Shared.ByteToHex(comByte: buffer);
            }
            catch (System.TimeoutException) { }
        }

        return "";
    }

    public void Write(byte[] buffer, int offset, int count) {
        if (IsOpened()) {
            try {
                Port?.Write(buffer: buffer, offset: offset, count: count);
            }
            catch (System.Exception) { }
        }
    }

    public void Write(string buffer) {
        byte[] bytes = WpfAppForModBus.Models.Helpers.Shared.HexToByte(message: buffer);

        Write(buffer: bytes, offset: 0, count: bytes.Length);
    }
}