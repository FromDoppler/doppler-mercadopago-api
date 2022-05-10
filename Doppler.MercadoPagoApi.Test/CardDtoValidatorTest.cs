using Xunit;
using FluentValidation.TestHelper;
using Doppler.MercadoPagoApi.Validators;

namespace Doppler.MercadoPagoApi
{
    public class CardDtoValidatorTest
    {
        private readonly CardDtoValidator _validator = new();

        [Theory]
        [InlineData("123")]
        [InlineData("1234")]
        public void Should_not_have_error_when_security_code_has_three_or_four_numbers(string securityCode)
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = securityCode };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("")]
        [InlineData(null)]
        public void Should_have_error_when_security_code_is_invalid_or_empty(string securityCode)
        {
            //Arrange
            var cardDto = new CardDto { SecurityCode = securityCode };

            //Act
            var result = _validator.TestValidate(cardDto);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Theory]
        [InlineData("5031755734530604")]
        [InlineData("")]
        [InlineData(null)]
        public void Should_have_error_when_card_number_is_empty(string cardNumber)
        {
            //Arrange
            var cardDto = new CardDto { CardNumber = cardNumber };

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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_have_error_when_expiration_month_is_empty(string expirationMonth)
        {
            //Arrange
            var cardDto = new CardDto { ExpirationMonth = expirationMonth };

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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_have_error_when_expiration_year_is_empty(string year)
        {
            //Arrange
            var cardDto = new CardDto { ExpirationYear = year };

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
