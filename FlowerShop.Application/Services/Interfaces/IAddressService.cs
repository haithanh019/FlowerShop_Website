using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IAddressService
    {
        IQueryable<AddressDTO> GetAddressesOData();
        Task<ApiResult<AddressDTO>> GetAddressByIDAsync(Guid id);
        Task<ApiResult<AddressDTO>> CreateAddressAsync(AddressCreateDTO dto);
        Task<ApiResult<AddressDTO>> UpdateAddressAsync(Guid id, AddressUpdateDTO dto);
        Task<ApiResult<bool>> DeleteAddressAsync(Guid id);
    }
}
