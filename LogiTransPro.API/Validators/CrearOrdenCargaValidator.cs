using FluentValidation;
using LogiTransPro.API.Models.DTOs.OrdenCarga;

namespace LogiTransPro.API.Validators
{
    public class CrearOrdenCargaValidator : AbstractValidator<CrearOrdenCargaDTO>
    {
        public CrearOrdenCargaValidator()
        {
            RuleFor(x => x.ClienteId)
                .GreaterThan(0).WithMessage("El cliente es requerido");

            RuleFor(x => x.FechaRequerida)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha requerida no puede ser anterior a hoy")
                .When(x => x.FechaRequerida.HasValue);

            RuleFor(x => x.DescripcionMercancia)
                .NotEmpty().WithMessage("La descripción de la mercancía es requerida")
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.PesoTotal)
                .NotEmpty().WithMessage("El peso total es requerido")
                .InclusiveBetween(0.01m, 100000).WithMessage("El peso debe estar entre 0.01 y 100,000 kg");

            RuleFor(x => x.VolumenTotal)
                .InclusiveBetween(0, 500).WithMessage("El volumen debe estar entre 0 y 500 metros cúbicos")
                .When(x => x.VolumenTotal.HasValue);

            RuleFor(x => x.InstruccionesEspeciales)
                .MaximumLength(1000).WithMessage("Las instrucciones especiales no pueden exceder 1000 caracteres");

            RuleFor(x => x.ValorDeclarado)
                .GreaterThanOrEqualTo(0).WithMessage("El valor declarado debe ser mayor o igual a 0")
                .When(x => x.ValorDeclarado.HasValue);

            RuleFor(x => x.Prioridad)
                .Must(x => x == "Normal" || x == "Alta" || x == "Urgente")
                .WithMessage("Prioridad debe ser Normal, Alta o Urgente");
        }
    }
}