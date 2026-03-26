using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTransPro.API.Models.Entities
{
    [Table("login_attempt", Schema = "seguridad")]
    public class LoginAttempt
    {
        [Key]
        [Column("login_attempt_id")]
        public int LoginAttemptId { get; set; }

        [Column("correo_electronico")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        [Column("exitoso")]
        public bool Exitoso { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }
    }
}