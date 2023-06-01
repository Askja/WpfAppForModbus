namespace WpfAppForModBus.Const;

public static class ParityList {
    public static WpfAppForModBus.Enums.ParityValues[] Parities { get; } = {
        new() { Name = "None", Type = System.IO.Ports.Parity.None },
        new() { Name = "Odd", Type = System.IO.Ports.Parity.Odd },
        new() { Name = "Even", Type = System.IO.Ports.Parity.Even }
    };
}