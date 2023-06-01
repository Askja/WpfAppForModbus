namespace WpfAppForModBus.Domain.Entities;

public class Sensor {
    [System.ComponentModel.DataAnnotations.KeyAttribute]
    public int SensorId { get; set; }

    [System.Diagnostics.CodeAnalysis.NotNullAttribute, System.ComponentModel.DataAnnotations.RequiredAttribute]
    public required string SensorName { get; set; }
}