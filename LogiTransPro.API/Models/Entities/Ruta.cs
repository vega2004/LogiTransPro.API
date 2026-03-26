using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("ruta", Schema = "logistica")]
    public class Ruta
    {
        [Key]
        [Column("ruta_id")]
        public int RutaId { get; set; }

        [Column("codigo_ruta")]
        public string CodigoRuta { get; set; } = string.Empty;

        [Column("nombre_ruta")]
        public string? NombreRuta { get; set; }

        [Column("origen")]
        public string Origen { get; set; } = string.Empty;

        [Column("destino")]
        public string Destino { get; set; } = string.Empty;

        [Column("distancia_estimada")]
        public decimal DistanciaEstimada { get; set; }

        [Column("tiempo_estimado")]
        public int? TiempoEstimado { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navigation properties
        public List<Viaje> Viajes { get; set; } = new();
    }
}