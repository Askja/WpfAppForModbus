namespace WpfAppForModBus.Const;

public static class StopBitsList {
    public static WpfAppForModBus.Enums.StopBitsValues[] StopBitsTypes { get; } = {
        new() { Name = "One", Type = System.IO.Ports.StopBits.One },
        new() { Name = "Two", Type = System.IO.Ports.StopBits.Two },
        new() { Name = "None", Type = System.IO.Ports.StopBits.None }
    };
}