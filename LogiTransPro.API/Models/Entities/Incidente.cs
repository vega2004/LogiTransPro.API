using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("incidente", Schema = "logistica")]
    public class Incidente
    {
        [Key]
        [Column("incidente_id")]
        public int IncidenteId { get; set; }

        [Column("viaje_id")]
        public int ViajeId { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        [Column("tipo_incidente")]
        public string TipoIncidente { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("gravedad")]
        public string? Gravedad { get; set; }

        [Column("reportado_por")]
        public int? ReportadoPor { get; set; }

        // Navigation properties
        [ForeignKey("ViajeId")]
        public Viaje? Viaje { get; set; }

        [ForeignKey("ReportadoPor")]
        public Usuario? ReportadoPorUsuario { get; set; }
    }
}