using FluentValidation;

namespace Doppler.MercadoPagoApi.Validators
{
    public class CardValidator: AbstractValidator<Card>
    {   
        public CardValidator()
        {
            RuleFor(x => x.SecurityCode).MaximumLength(4).NotEmpty();
            RuleFor(x => x.CardNumber).CreditCard().NotEmpty();
        }
    }
}
