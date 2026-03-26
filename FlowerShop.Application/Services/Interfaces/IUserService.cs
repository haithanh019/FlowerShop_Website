using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IUserService
    {
        IQueryable<UserDTO> GetUsersOData();
        Task<ApiResult<UserDTO>> GetUserByIDAsync(Guid id);
        Task<ApiResult<UserDTO>> RegisterAsync(UserRegisterDTO dto);
        Task<ApiResult<UserDTO>> LoginAsync(UserLoginDTO dto);
        Task<ApiResult<bool>> LogoutAsync();
    }
}
