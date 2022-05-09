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
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { Installments = 5 };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_not_have_error_when_installments_are_one()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { Installments = 1 };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_have_error_when_installments_are_empty()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_less_than_zero()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = -1 };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_zero()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 0 };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_not_have_error_when_transaction_amount_is_more_than_zero()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 10 };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_trnsaction_amount_is_empty()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_visa()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "visa" };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_master()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "master" };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_not_have_error_when_payment_method_id_is_amex()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "amex" };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_have_error_when_payment_method_id_is_invalid()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "dopplercard" };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Fact]
        public void Should_have_error_when_payment_method_id_is_empty()
        {
            //Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = "" };

            //Act
            var result = _validator.TestValidate(paymentRequestDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
        }
    }
}
