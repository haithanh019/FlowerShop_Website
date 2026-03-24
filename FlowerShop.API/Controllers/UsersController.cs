using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class UsersController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        [EnableQuery]
        public IQueryable<UserDTO> Get()
        {
            return _facadeService.UserService.GetUsersOData();
        }
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.UserService.GetUserByIDAsync(key);
            return Ok(result);
        }
        [HttpPost("api/Users/Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var result = await _facadeService.UserService.RegisterAsync(dto);
            return Ok(result);
        }
        [HttpPost("api/Users/Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var result = await _facadeService.UserService.LoginAsync(dto);
            return Ok(result);
        }
        [HttpPost("api/Users/Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _facadeService.UserService.LogoutAsync();
            return Ok(result);
        }
    }
}
