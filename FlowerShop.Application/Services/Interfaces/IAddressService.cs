using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IAddressService
    {
        IQueryable<AddressDTO> GetAddressesOData();
        Task<ApiResponse<AddressDTO>> GetAddressByIDAsync(Guid id);
        Task<ApiResponse<AddressDTO>> CreateAddressAsync(AddressCreateDTO dto);
        Task<ApiResponse<AddressDTO>> UpdateAddressAsync(Guid id, AddressUpdateDTO dto);
        Task<ApiResponse<bool>> DeleteAddressAsync(Guid id);
    }
}
