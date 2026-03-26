using FluentValidation;
using LogiTransPro.API.Models.DTOs.Ruta;

namespace LogiTransPro.API.Validators
{
    public class OptimizarRutaValidator : AbstractValidator<OptimizarRutaDTO>
    {
        public OptimizarRutaValidator()
        {
            RuleFor(x => x.Origen)
                .NotEmpty().WithMessage("El origen es requerido")
                .MaximumLength(150).WithMessage("El origen no puede exceder 150 caracteres");

            RuleFor(x => x.Destino)
                .NotEmpty().WithMessage("El destino es requerido")
                .MaximumLength(150).WithMessage("El destino no puede exceder 150 caracteres")
                .NotEqual(x => x.Origen).WithMessage("El origen y destino no pueden ser iguales");

            RuleFor(x => x.TipoOptimizacion)
                .Must(x => x == "distancia" || x == "tiempo")
                .WithMessage("Tipo de optimización debe ser 'distancia' o 'tiempo'")
                .When(x => !string.IsNullOrEmpty(x.TipoOptimizacion));

            RuleForEach(x => x.PuntosIntermedios)
                .MaximumLength(150).WithMessage("Cada punto intermedio no puede exceder 150 caracteres")
                .When(x => x.PuntosIntermedios != null);
        }
    }
}