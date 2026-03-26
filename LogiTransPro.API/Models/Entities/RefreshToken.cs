using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("refresh_token", Schema = "seguridad")]
    public class RefreshToken
    {
        [Key]
        [Column("refresh_token_id")]
        public int RefreshTokenId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("revocado")]
        public bool Revocado { get; set; }

        [Column("dispositivo")]
        public string? Dispositivo { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        // Navigation properties
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}