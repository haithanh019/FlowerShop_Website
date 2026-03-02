using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API
{
    public class UsersController : ODataController
    {
        private readonly IFacadeService _facadeService;
        public UsersController(IFacadeService facadeService)
        {
            _facadeService = facadeService;
        }

        [EnableQuery]
        public IQueryable<UserDTO> Get()
        {
            return _facadeService.UserService.GetUsersOData();
        }

        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var result = await _facadeService.UserService.RegisterAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var result = await _facadeService.UserService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
