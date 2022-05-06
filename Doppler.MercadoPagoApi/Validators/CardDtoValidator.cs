using FluentValidation;

namespace Doppler.MercadoPagoApi.Validators
{
    public class CardDtoValidator: AbstractValidator<CardDto>
    {   
        public CardDtoValidator()
        {
            RuleFor(x => x.SecurityCode).MaximumLength(4).MinimumLength(3).NotEmpty().WithMessage("Invalid security code");
            RuleFor(x => x.CardNumber).CreditCard().NotEmpty().WithMessage("Invalid card number");
        }
    }
}
