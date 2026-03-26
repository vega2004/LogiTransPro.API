using FluentValidation;
using LogiTransPro.API.Models.DTOs.Mantenimiento;

namespace LogiTransPro.API.Validators
{
    public class CrearMantenimientoValidator : AbstractValidator<CrearMantenimientoDTO>
    {
        public CrearMantenimientoValidator()
        {
            RuleFor(x => x.VehiculoId)
                .GreaterThan(0).WithMessage("El vehículo es requerido");

            RuleFor(x => x.TipoServicio)
                .NotEmpty().WithMessage("El tipo de servicio es requerido")
                .Must(x => x == "P" || x == "C")
                .WithMessage("Tipo servicio debe ser P (Preventivo) o C (Correctivo)");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.FechaProgramada)
                .NotEmpty().WithMessage("La fecha programada es requerida")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha programada no puede ser anterior a hoy");

            RuleFor(x => x.KilometrajeAlerta)
                .GreaterThanOrEqualTo(0).WithMessage("El kilometraje de alerta debe ser mayor o igual a 0")
                .When(x => x.KilometrajeAlerta.HasValue);

            RuleFor(x => x.Costo)
                .GreaterThanOrEqualTo(0).WithMessage("El costo debe ser mayor o igual a 0")
                .When(x => x.Costo.HasValue);

            RuleFor(x => x.Prioridad)
                .NotEmpty().WithMessage("La prioridad es requerida")
                .Must(x => x == "Baja" || x == "Media" || x == "Alta" || x == "Critica")
                .WithMessage("Prioridad debe ser Baja, Media, Alta o Critica");

            RuleFor(x => x.TecnicoAsignado)
                .MaximumLength(100).WithMessage("El técnico asignado no puede exceder 100 caracteres");

            // Validar cada parte usando la clase anidada CrearParteMantenimientoDTO
            RuleForEach(x => x.Partes).ChildRules(parte =>
            {
                parte.RuleFor(p => p.NombreParte)
                    .NotEmpty().WithMessage("El nombre de la parte es requerido")
                    .MaximumLength(100).WithMessage("El nombre de la parte no puede exceder 100 caracteres");

                parte.RuleFor(p => p.Cantidad)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0")
                    .LessThanOrEqualTo(999).WithMessage("La cantidad no puede exceder 999");

                parte.RuleFor(p => p.CostoUnitario)
                    .GreaterThanOrEqualTo(0).WithMessage("El costo unitario debe ser mayor o igual a 0")
                    .When(p => p.CostoUnitario.HasValue);
            });
        }
    }
}