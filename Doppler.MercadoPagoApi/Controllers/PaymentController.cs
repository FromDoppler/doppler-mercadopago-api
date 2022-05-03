using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppler.MercadoPagoApi.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentController : Controller
    {
        public PaymentController()
        {

        }
    }
}
