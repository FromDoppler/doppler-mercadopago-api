using FluentValidation;

namespace Doppler.MercadoPagoApi.Validators
{
    public class PaymentRequestDtoValidator : AbstractValidator<PaymentRequestDto>
    {
        public PaymentRequestDtoValidator()
        {
            RuleFor(x => x.TransactionAmount).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(x => x.Installments).NotEmpty().Equal(1);
            RuleFor(x => x.PaymentMethodId).NotEmpty();
        }
    }
}
