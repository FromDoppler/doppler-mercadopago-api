using FluentValidation;

namespace Doppler.MercadoPagoApi.Validators
{
    public class PaymentRequestDtoValidator : AbstractValidator<PaymentRequestDto>
    {
        public PaymentRequestDtoValidator()
        {
            RuleFor(x => x.TransactionAmount).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Invalid or empty Installments");
            RuleFor(x => x.Installments).NotEmpty().Equal(1).WithMessage("Invalid or empty Installments");
            RuleFor(x => x.PaymentMethodId).NotEmpty().WithMessage("Invalid or empty PaymentMethodId").Custom((element, context) =>
            {
                if (element != "amex" && element != "visa" && element != "master")
                {
                    context.AddFailure("Invalid PaymentMethodId");
                }
            });
        }
    }
}
