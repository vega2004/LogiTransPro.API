namespace LogiTransPro.API.Models.DTOs.Cliente
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public string Rfc { get; set; } = string.Empty;
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; }
        public string? PersonaContacto { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}