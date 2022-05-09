using Xunit;
using FluentValidation.TestHelper;
using Doppler.MercadoPagoApi.Validators;

namespace Doppler.MercadoPagoApi
{
    public class PaymentRequestDtoValidatorTest
    {
        private readonly PaymentRequestDtoValidator _validator = new();

        [Fact]
        public void Should_have_error_when_installments_are_different_than_one()
        {
            var paymentRequestDto = new PaymentRequestDto { Installments = 5 };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_not_have_error_when_installments_are_one()
        {
            var paymentRequestDto = new PaymentRequestDto { Installments = 1 };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldNotHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_have_error_when_installments_are_empty()
        {
            var paymentRequestDto = new PaymentRequestDto { };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_less_than_zero()
        {
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = -1 };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_zero()
        {
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 0 };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_not_have_error_when_transaction_amount_is_more_than_zero()
        {
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 10 };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldNotHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_trnsaction_amount_is_empty()
        {
            var paymentRequestDto = new PaymentRequestDto { };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_visa()
        {
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "visa" };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_master()
        {
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "master" };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_amex()
        {
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "amex" };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_have_error_when_payment_method_id_is_invalid()
        {
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "dopplercard" };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_have_error_when_payment_method_id_is_empty()
        {
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "" };
            var result = _validator.TestValidate(paymentRequestDto);
            result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
        }
    }
}
