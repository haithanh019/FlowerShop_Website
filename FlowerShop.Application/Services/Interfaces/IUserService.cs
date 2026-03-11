using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IUserService
    {
        IQueryable<UserDTO> GetUsersOData();
        Task<ApiResponse<UserDTO>> GetUserByIDAsync(Guid id);
        Task<ApiResponse<UserDTO>> RegisterAsync(UserRegisterDTO dto);
        Task<ApiResponse<UserDTO>> LoginAsync(UserLoginDTO dto);
        Task<ApiResponse<bool>> LogoutAsync();
    }
}
