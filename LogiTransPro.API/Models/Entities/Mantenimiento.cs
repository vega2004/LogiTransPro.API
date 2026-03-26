using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("mantenimiento", Schema = "flotilla")]
    public class Mantenimiento
    {
        [Key]
        [Column("mantenimiento_id")]
        public int MantenimientoId { get; set; }

        [Column("vehiculo_id")]
        public int VehiculoId { get; set; }

        [Column("tipo_servicio")]
        public string TipoServicio { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_programada")]
        public DateTime FechaProgramada { get; set; }

        [Column("fecha_realizada")]
        public DateTime? FechaRealizada { get; set; }

        [Column("kilometraje_alerta")]
        public int? KilometrajeAlerta { get; set; }

        [Column("kilometraje_actual")]
        public int? KilometrajeActual { get; set; }

        [Column("costo")]
        public decimal? Costo { get; set; }

        [Column("prioridad")]
        public string Prioridad { get; set; } = "Media";

        [Column("estatus")]
        public string Estatus { get; set; } = "P";

        [Column("notas_mecanico")]
        public string? NotasMecanico { get; set; }

        [Column("tecnico_asignado")]
        public string? TecnicoAsignado { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("VehiculoId")]
        public Vehiculo? Vehiculo { get; set; }

        public List<ParteMantenimiento> Partes { get; set; } = new();
    }
}