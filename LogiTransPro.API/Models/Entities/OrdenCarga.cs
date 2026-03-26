using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("orden_carga", Schema = "comercial")]
    public class OrdenCarga
    {
        [Key]
        [Column("orden_carga_id")]
        public int OrdenCargaId { get; set; }

        [Column("numero_orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("fecha_solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        [Column("fecha_requerida")]
        public DateTime? FechaRequerida { get; set; }

        [Column("descripcion_mercancia")]
        public string DescripcionMercancia { get; set; } = string.Empty;

        [Column("peso_total")]
        public decimal PesoTotal { get; set; }

        [Column("volumen_total")]
        public decimal? VolumenTotal { get; set; }

        [Column("instrucciones_especiales")]
        public string? InstruccionesEspeciales { get; set; }

        [Column("valor_declarado")]
        public decimal? ValorDeclarado { get; set; }

        [Column("estatus")]
        public string Estatus { get; set; } = "P";

        [Column("prioridad")]
        public string Prioridad { get; set; } = "Normal";

        // Navigation properties
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public List<Viaje> Viajes { get; set; } = new();
    }
}