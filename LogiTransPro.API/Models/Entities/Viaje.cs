using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("viaje", Schema = "logistica")]
    public class Viaje
    {
        [Key]
        [Column("viaje_id")]
        public int ViajeId { get; set; }

        [Column("numero_viaje")]
        public string NumeroViaje { get; set; } = string.Empty;

        [Column("orden_carga_id")]
        public int OrdenCargaId { get; set; }

        [Column("vehiculo_id")]
        public int VehiculoId { get; set; }

        [Column("chofer_id")]
        public int ChoferId { get; set; }

        [Column("ruta_id")]
        public int RutaId { get; set; }

        [Column("fecha_salida_programada")]
        public DateTime FechaSalidaProgramada { get; set; }

        [Column("fecha_llegada_programada")]
        public DateTime? FechaLlegadaProgramada { get; set; }

        [Column("fecha_salida_real")]
        public DateTime? FechaSalidaReal { get; set; }

        [Column("fecha_llegada_real")]
        public DateTime? FechaLlegadaReal { get; set; }

        [Column("kilometraje_inicial")]
        public int? KilometrajeInicial { get; set; }

        [Column("kilometraje_final")]
        public int? KilometrajeFinal { get; set; }

        [Column("consumo_combustible")]
        public decimal? ConsumoCombustible { get; set; }

        [Column("estatus")]
        public string Estatus { get; set; } = "P";

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("OrdenCargaId")]
        public OrdenCarga? OrdenCarga { get; set; }

        [ForeignKey("VehiculoId")]
        public Vehiculo? Vehiculo { get; set; }

        [ForeignKey("ChoferId")]
        public Usuario? Chofer { get; set; }

        [ForeignKey("RutaId")]
        public Ruta? Ruta { get; set; }

        public List<Incidente> Incidentes { get; set; } = new();
    }
}