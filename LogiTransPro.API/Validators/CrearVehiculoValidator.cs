using FluentValidation;
using LogiTransPro.API.Models.DTOs.Vehiculo;

namespace LogiTransPro.API.Validators
{
    public class CrearVehiculoValidator : AbstractValidator<CrearVehiculoDTO>
    {
        public CrearVehiculoValidator()
        {
            RuleFor(x => x.Placa)
                .NotEmpty().WithMessage("La placa es requerida")
                .Length(5, 10).WithMessage("La placa debe tener entre 5 y 10 caracteres")
                .Matches(@"^[A-Z0-9]+$").WithMessage("La placa solo puede contener letras mayúsculas y números");

            RuleFor(x => x.Vin)
                .NotEmpty().WithMessage("El VIN es requerido")
                .Length(17).WithMessage("El VIN debe tener exactamente 17 caracteres")
                .Matches(@"^[A-HJ-NPR-Z0-9]{17}$").WithMessage("El VIN tiene formato inválido");

            RuleFor(x => x.Marca)
                .NotEmpty().WithMessage("La marca es requerida")
                .MaximumLength(50).WithMessage("La marca no puede exceder 50 caracteres");

            RuleFor(x => x.Modelo)
                .NotEmpty().WithMessage("El modelo es requerido")
                .MaximumLength(50).WithMessage("El modelo no puede exceder 50 caracteres");

            RuleFor(x => x.Anio)
                .NotEmpty().WithMessage("El año es requerido")
                .InclusiveBetween(1980, DateTime.UtcNow.Year + 1)
                .WithMessage($"El año debe estar entre 1980 y {DateTime.UtcNow.Year + 1}");

            RuleFor(x => x.CapacidadCarga)
                .NotEmpty().WithMessage("La capacidad de carga es requerida")
                .InclusiveBetween(0.01m, 100).WithMessage("La capacidad de carga debe estar entre 0.01 y 100 toneladas");

            RuleFor(x => x.CapacidadVolumen)
                .InclusiveBetween(0, 500).WithMessage("El volumen debe estar entre 0 y 500 metros cúbicos")
                .When(x => x.CapacidadVolumen.HasValue);
        }
    }
}