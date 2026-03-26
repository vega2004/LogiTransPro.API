using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("parte_mantenimiento", Schema = "flotilla")]
    public class ParteMantenimiento
    {
        [Key]
        [Column("parte_id")]
        public int ParteId { get; set; }

        [Column("mantenimiento_id")]
        public int MantenimientoId { get; set; }

        [Column("nombre_parte")]
        public string NombreParte { get; set; } = string.Empty;

        [Column("cantidad")]
        public int Cantidad { get; set; } = 1;

        [Column("costo_unitario")]
        public decimal? CostoUnitario { get; set; }

        // Navigation properties
        [ForeignKey("MantenimientoId")]
        public Mantenimiento? Mantenimiento { get; set; }
    }
}