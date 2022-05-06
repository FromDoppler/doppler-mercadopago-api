using System.ComponentModel.DataAnnotations;

namespace Doppler.MercadoPagoApi.Models
{
    public class CustomerDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
