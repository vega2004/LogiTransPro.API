using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("usuario", Schema = "seguridad")]
    public class Usuario
    {
        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("correo_electronico")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Column("contrasena_hash")]
        public string ContrasenaHash { get; set; } = string.Empty;

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "A";

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        [Column("ultimo_cambio_password")]
        public DateTime? UltimoCambioPassword { get; set; }

        [Column("requiere_cambio_password")]
        public bool RequiereCambioPassword { get; set; }

        [Column("intentos_fallidos")]
        public int IntentosFallidos { get; set; }

        [Column("bloqueado_hasta")]
        public DateTime? BloqueadoHasta { get; set; }

        // Navigation properties
        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public List<Viaje> ViajesAsignados { get; set; } = new();
        public List<Incidente> IncidentesReportados { get; set; } = new();
    }
}