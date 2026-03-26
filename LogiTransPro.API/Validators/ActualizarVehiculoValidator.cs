using FluentValidation;
using LogiTransPro.API.Models.DTOs.Vehiculo;

namespace LogiTransPro.API.Validators  // ← Sin .Vehiculo
{
    public class ActualizarVehiculoValidator : AbstractValidator<ActualizarVehiculoDTO>
    {
        public ActualizarVehiculoValidator()
        {
            RuleFor(x => x.Placa)
                .Length(5, 10).WithMessage("La placa debe tener entre 5 y 10 caracteres")
                .Matches(@"^[A-Z0-9]+$").WithMessage("La placa solo puede contener letras mayúsculas y números")
                .When(x => !string.IsNullOrEmpty(x.Placa));

            RuleFor(x => x.Vin)
                .Length(17).WithMessage("El VIN debe tener exactamente 17 caracteres")
                .Matches(@"^[A-HJ-NPR-Z0-9]{17}$").WithMessage("El VIN tiene formato inválido")
                .When(x => !string.IsNullOrEmpty(x.Vin));

            RuleFor(x => x.Marca)
                .MaximumLength(50).WithMessage("La marca no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Marca));

            RuleFor(x => x.Modelo)
                .MaximumLength(50).WithMessage("El modelo no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Modelo));

            RuleFor(x => x.Anio)
                .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
                .WithMessage($"El año debe estar entre 1980 y {DateTime.UtcNow.Year + 1}")
                .When(x => x.Anio.HasValue);

            RuleFor(x => x.CapacidadCarga)
                .InclusiveBetween(0.01m, 100).WithMessage("La capacidad de carga debe estar entre 0.01 y 100 toneladas")
                .When(x => x.CapacidadCarga.HasValue);

            RuleFor(x => x.CapacidadVolumen)
                .InclusiveBetween(0, 500).WithMessage("El volumen debe estar entre 0 y 500 metros cúbicos")
                .When(x => x.CapacidadVolumen.HasValue);

            RuleFor(x => x.KilometrajeActual)
                .GreaterThanOrEqualTo(0).WithMessage("El kilometraje debe ser mayor o igual a 0")
                .When(x => x.KilometrajeActual.HasValue);

            RuleFor(x => x.NivelCombustible)
                .InclusiveBetween(0, 100).WithMessage("El nivel de combustible debe estar entre 0 y 100%")
                .When(x => x.NivelCombustible.HasValue);

            RuleFor(x => x.EstadoGeneral)
                .Must(x => x == "D" || x == "R" || x == "M")
                .WithMessage("Estado general debe ser D (Disponible), R (En Ruta) o M (Mantenimiento)")
                .When(x => !string.IsNullOrEmpty(x.EstadoGeneral));
        }
    }
}