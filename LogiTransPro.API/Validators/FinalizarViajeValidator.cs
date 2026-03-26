using FluentValidation;
using LogiTransPro.API.Models.DTOs.Viaje;

namespace LogiTransPro.API.Validators
{
    public class FinalizarViajeValidator : AbstractValidator<FinalizarViajeDTO>
    {
        public FinalizarViajeValidator()
        {
            RuleFor(x => x.KilometrajeFinal)
                .GreaterThanOrEqualTo(0).WithMessage("El kilometraje final debe ser mayor o igual a 0")
                .LessThanOrEqualTo(999999).WithMessage("El kilometraje no puede exceder 999,999 km");

            RuleFor(x => x.ConsumoCombustible)
                .GreaterThanOrEqualTo(0).WithMessage("El consumo de combustible debe ser mayor o igual a 0")
                .LessThanOrEqualTo(9999).WithMessage("El consumo no puede exceder 9,999 litros");
        }
    }
}