using FluentValidation;
using LogiTransPro.API.Models.DTOs.Auth;

namespace LogiTransPro.API.Validators
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es requerido")
                .EmailAddress().WithMessage("Formato de correo electrónico inválido")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres");

            RuleFor(x => x.Contrasena)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres");
        }
    }
}