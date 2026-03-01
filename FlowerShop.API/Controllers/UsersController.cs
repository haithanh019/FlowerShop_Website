using FlowerShop.Application;
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
    }
}
