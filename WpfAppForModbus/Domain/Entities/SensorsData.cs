using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WpfAppForModbus.Domain.Entities {
    public class SensorsData {
        [Key]
        public Guid RowId {
            get; set;
        }

        [ForeignKey(nameof(Sensor))]
        public int SensorId {
            get; set;
        }

        [NotNull]
        [Required]
        public required string SensorData {
            get; set;
        }

        [NotNull]
        public DateTime RowDate {
            get; set;
        }
    }
}
