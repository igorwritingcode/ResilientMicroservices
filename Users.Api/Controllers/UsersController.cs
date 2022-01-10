using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Users.Api.Models;
using Users.Api.Services;

namespace Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        readonly IAddressService _addressService;

        public UsersController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var userAddress = await _addressService.GetAddress(userId);
            
            var user = new User()
            {
                UserId = userId,
                Address = userAddress,
                UserName = "SomeUserName"
            };

            return Ok(user);
        }
    }
}
