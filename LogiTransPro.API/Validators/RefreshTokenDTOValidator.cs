using FluentValidation;
using LogiTransPro.API.Models.DTOs.Auth;

namespace LogiTransPro.API.Validators
{
    public class RefreshTokenDTOValidator : AbstractValidator<RefreshTokenDTO>
    {
        public RefreshTokenDTOValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El refresh token es requerido");
        }
    }
}