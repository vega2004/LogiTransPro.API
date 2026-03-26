using FluentValidation;
using LogiTransPro.API.Models.DTOs.Cliente;

namespace LogiTransPro.API.Validators
{
    public class CrearClienteValidator : AbstractValidator<CrearClienteDTO>
    {
        public CrearClienteValidator()
        {
            RuleFor(x => x.Rfc)
                .NotEmpty().WithMessage("El RFC es requerido")
                .Length(12, 13).WithMessage("El RFC debe tener 12 o 13 caracteres")
                .Matches(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$")
                .WithMessage("Formato de RFC inválido");

            RuleFor(x => x.NombreRazonSocial)
                .NotEmpty().WithMessage("La razón social es requerida")
                .MaximumLength(150).WithMessage("La razón social no puede exceder 150 caracteres");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
                .Matches(@"^[0-9+\-\s]+$").WithMessage("Formato de teléfono inválido");

            RuleFor(x => x.CorreoElectronico)
                .EmailAddress().WithMessage("Formato de correo electrónico inválido")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.CorreoElectronico));

            RuleFor(x => x.PersonaContacto)
                .MaximumLength(100).WithMessage("La persona de contacto no puede exceder 100 caracteres");

            RuleFor(x => x.Direccion)
                .MaximumLength(255).WithMessage("La dirección no puede exceder 255 caracteres");
        }
    }
}