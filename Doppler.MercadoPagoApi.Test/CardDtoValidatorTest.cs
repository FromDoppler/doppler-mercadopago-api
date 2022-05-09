using Xunit;
using FluentValidation.TestHelper;
using Doppler.MercadoPagoApi.Validators;

namespace Doppler.MercadoPagoApi
{
    public class CardDtoValidatorTest
    {
        private readonly CardDtoValidator _validator = new();

        [Fact]
        public void Should_not_have_error_when_security_code_has_three_numbers()
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = "123" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_not_have_error_when_security_code_has_four_numbers()
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = "1234" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_security_code_has_five_numbers()
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = "12345" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_security_code_is_empty()
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = "" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_card_number_is_empty()
        {
            //Arrange
            var cardDto = new CardDto { CardNumber = "" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_have_error_when_card_number_is_invalid()
        {
            //Arrange
            var cardDto = new CardDto { CardNumber = "4588999" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_not_have_error_when_card_number_is_valid()
        {
            //Arrange
            var cardDto = new CardDto { CardNumber = "5031755734530604" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_have_error_when_expiration_month_is_empty()
        {
            //Arrange
            var cardDto = new CardDto { ExpirationMonth = "" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpirationMonth);
        }

        [Fact]
        public void Should_not_have_error_when_expiration_month_is_not_empty()
        {
            //Arrange
            var cardDto = new CardDto { ExpirationMonth = "10" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationMonth);
        }

        [Fact]
        public void Should_have_error_when_expiration_year_is_empty()
        {
            //Arrange
            var cardDto = new CardDto { ExpirationYear = "" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.ExpirationYear);
        }

        [Fact]
        public void Should_not_have_error_when_expiration_year_is_not_empty()
        {
            //Arrange
            var cardDto = new CardDto { ExpirationYear = "2026" };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationYear);
        }
    }
}
