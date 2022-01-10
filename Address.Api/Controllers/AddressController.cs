using Address.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Address.Api.Controllers
{
    [ApiController]
    [Route("[controller]/addresses")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAddressByUserId(int userId)
        {
            try
            {
                var address = await _addressService.GetUserAddress(userId);
                return Ok(address);
            }
            catch
            {
                return StatusCode(503);
            }
        }
    }
}
