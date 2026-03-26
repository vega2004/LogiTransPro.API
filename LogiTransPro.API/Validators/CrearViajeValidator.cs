using FluentValidation;
using LogiTransPro.API.Models.DTOs.Viaje;

namespace LogiTransPro.API.Validators
{
    public class CrearViajeValidator : AbstractValidator<CrearViajeDTO>
    {
        public CrearViajeValidator()
        {
            RuleFor(x => x.OrdenCargaId)
                .GreaterThan(0).WithMessage("La orden de carga es requerida");

            RuleFor(x => x.VehiculoId)
                .GreaterThan(0).WithMessage("El vehículo es requerido");

            RuleFor(x => x.ChoferId)
                .GreaterThan(0).WithMessage("El chofer es requerido");

            RuleFor(x => x.RutaId)
                .GreaterThan(0).WithMessage("La ruta es requerida");

            RuleFor(x => x.FechaSalidaProgramada)
                .NotEmpty().WithMessage("La fecha de salida programada es requerida")
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("La fecha de salida no puede ser anterior a la fecha actual");

            RuleFor(x => x.FechaLlegadaProgramada)
                .GreaterThan(x => x.FechaSalidaProgramada)
                .WithMessage("La fecha de llegada debe ser posterior a la fecha de salida")
                .When(x => x.FechaLlegadaProgramada.HasValue);

            RuleFor(x => x.Observaciones)
                .MaximumLength(500).WithMessage("Las observaciones no pueden exceder 500 caracteres");
        }
    }
}