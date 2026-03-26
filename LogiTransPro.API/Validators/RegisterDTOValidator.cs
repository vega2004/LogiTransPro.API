using FluentValidation;
using LogiTransPro.API.Models.DTOs.Auth;

namespace LogiTransPro.API.Validators
{
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.NombreCompleto)
                .NotEmpty().WithMessage("El nombre completo es requerido")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es requerido")
                .EmailAddress().WithMessage("Formato de correo electrónico inválido")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres");

            RuleFor(x => x.Contrasena)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
                .WithMessage("La contraseña debe tener al menos una mayúscula, una minúscula y un número");

            RuleFor(x => x.ConfirmarContrasena)
                .NotEmpty().WithMessage("Confirmar contraseña es requerido")
                .Equal(x => x.Contrasena).WithMessage("Las contraseñas no coinciden");

            RuleFor(x => x.Telefono)
                .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
                .Matches(@"^[0-9+\-\s]+$").WithMessage("Formato de teléfono inválido")
                .When(x => !string.IsNullOrEmpty(x.Telefono));

            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("El rol debe ser válido")
                .When(x => x.RolId.HasValue);
        }
    }
}