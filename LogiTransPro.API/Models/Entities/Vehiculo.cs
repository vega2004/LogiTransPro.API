using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("vehiculo", Schema = "flotilla")]
    public class Vehiculo
    {
        [Key]
        [Column("vehiculo_id")]
        public int VehiculoId { get; set; }

        [Column("placa")]
        public string Placa { get; set; } = string.Empty;

        [Column("vin")]
        public string Vin { get; set; } = string.Empty;

        [Column("marca")]
        public string Marca { get; set; } = string.Empty;

        [Column("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [Column("anio")]
        public int Anio { get; set; }

        [Column("capacidad_carga")]
        public decimal CapacidadCarga { get; set; }

        [Column("capacidad_volumen")]
        public decimal? CapacidadVolumen { get; set; }

        [Column("kilometraje_actual")]
        public int KilometrajeActual { get; set; }

        [Column("nivel_combustible")]
        public decimal NivelCombustible { get; set; } = 100;

        [Column("estado_motor")]
        public string EstadoMotor { get; set; } = "Óptimo";

        [Column("estado_general")]
        public string EstadoGeneral { get; set; } = "D";

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navigation properties
        public List<Mantenimiento> Mantenimientos { get; set; } = new();
        public List<Viaje> Viajes { get; set; } = new();
    }
}