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
            var cardDto = new CardDto { SecurityCode = "123" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldNotHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_not_have_error_when_security_code_has_four_numbers()
        {
            var cardDto = new CardDto { SecurityCode = "1234" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldNotHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_security_code_has_five_numbers()
        {
            var cardDto = new CardDto { SecurityCode = "12345" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_security_code_is_empty()
        {
            var cardDto = new CardDto { SecurityCode = "" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.SecurityCode);
        }

        [Fact]
        public void Should_have_error_when_card_number_is_empty()
        {
            var cardDto = new CardDto { CardNumber = "" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_have_error_when_card_number_is_invalid()
        {
            var cardDto = new CardDto { CardNumber = "4588999" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_not_have_error_when_card_number_is_valid()
        {
            var cardDto = new CardDto { CardNumber = "5031755734530604" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_have_error_when_expiration_month_is_empty()
        {
            var cardDto = new CardDto { ExpirationMonth = "" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.ExpirationMonth);
        }

        [Fact]
        public void Should_not_have_error_when_expiration_month_is_not_empty()
        {
            var cardDto = new CardDto { ExpirationMonth = "10" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationMonth);
        }

        [Fact]
        public void Should_have_error_when_expiration_year_is_empty()
        {
            var cardDto = new CardDto { ExpirationYear = "" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldHaveValidationErrorFor(x => x.ExpirationYear);
        }

        [Fact]
        public void Should_have_not_error_when_expiration_year_is_not_empty()
        {
            var cardDto = new CardDto { ExpirationYear = "2026" };
            var result = _validator.TestValidate(cardDto);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpirationYear);
        }
    }
}
