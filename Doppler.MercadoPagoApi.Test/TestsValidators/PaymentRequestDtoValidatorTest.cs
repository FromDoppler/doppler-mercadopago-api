using Xunit;
using FluentValidation.TestHelper;
using Doppler.MercadoPagoApi.Validators;

namespace Doppler.MercadoPagoApi
{
    public class PaymentRequestDtoValidatorTest
    {
        private readonly PaymentRequestDtoValidator _validator = new();

        [Theory]
        [InlineData(5)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_have_error_when_installments_are_different_than_one(int quantity)
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { Installments = quantity };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_not_have_error_when_installments_are_one()
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { Installments = 1 };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Installments);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_less_than_zero()
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = -1 };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_transaction_amount_is_zero()
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 0 };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_not_have_error_when_transaction_amount_is_more_than_zero()
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { TransactionAmount = 10 };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Fact]
        public void Should_have_error_when_trnsaction_amount_is_empty()
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TransactionAmount);
        }

        [Theory]
        [InlineData("visa")]
        [InlineData("master")]
        [InlineData("amex")]
        public void Should_not_have_error_when_payment_method_id_is_valid(string method)
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = method };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethodId);
        }

        [Theory]
        [InlineData("dopplercard")]
        [InlineData("")]
        [InlineData(null)]
        public void Should_have_error_when_payment_method_id_is_invalid_or_empty(string method)
        {
            // Arrange
            var paymentRequestDto = new PaymentRequestDto { PaymentMethodId = method };

            // Act
            var result = _validator.TestValidate(paymentRequestDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
        }
    }
}
