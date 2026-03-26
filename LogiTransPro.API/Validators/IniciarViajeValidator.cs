using FluentValidation;
using LogiTransPro.API.Models.DTOs.Viaje;

namespace LogiTransPro.API.Validators
{
    public class IniciarViajeValidator : AbstractValidator<IniciarViajeDTO>
    {
        public IniciarViajeValidator()
        {
            RuleFor(x => x.KilometrajeInicial)
                .GreaterThanOrEqualTo(0).WithMessage("El kilometraje inicial debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999).WithMessage("El kilometraje no puede exceder 999,999 km");

            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("El ID del usuario es requerido");
        }
    }
}