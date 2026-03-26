using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("cliente", Schema = "comercial")]
    public class Cliente
    {
        [Key]
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("rfc")]
        public string Rfc { get; set; } = string.Empty;

        [Column("nombre_razon_social")]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("correo_electronico")]
        public string? CorreoElectronico { get; set; }

        [Column("persona_contacto")]
        public string? PersonaContacto { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navigation properties
        public List<OrdenCarga> OrdenesCarga { get; set; } = new();
    }
}