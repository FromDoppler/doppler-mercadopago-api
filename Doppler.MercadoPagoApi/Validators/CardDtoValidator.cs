using FluentValidation;

namespace Doppler.MercadoPagoApi.Validators
{
    public class CardDtoValidator: AbstractValidator<CardDto>
    {   
        public CardDtoValidator()
        {
            RuleFor(x => x.SecurityCode).MaximumLength(4).NotEmpty();
            RuleFor(x => x.CardNumber).CreditCard().NotEmpty();
        }
    }
}
