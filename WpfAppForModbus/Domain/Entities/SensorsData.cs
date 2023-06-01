namespace WpfAppForModBus.Domain.Entities;

public class SensorsData {
    [System.ComponentModel.DataAnnotations.KeyAttribute]
    public System.Guid RowId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute(name: nameof(Sensor))]
    public int SensorId { get; set; }

    [System.Diagnostics.CodeAnalysis.NotNullAttribute, System.ComponentModel.DataAnnotations.RequiredAttribute]
    public required string SensorData { get; set; }

    [System.Diagnostics.CodeAnalysis.NotNullAttribute]
    public System.DateTime RowDate { get; set; }
}