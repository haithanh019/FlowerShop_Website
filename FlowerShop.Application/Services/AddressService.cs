using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IQueryable<AddressDTO> GetAddressesOData()
        {
            return _unitOfWork.AddressRepository.GetQuery().ProjectTo<AddressDTO>(_mapper.ConfigurationProvider);
        }
        public async Task<ApiResult<AddressDTO>> GetAddressByIDAsync(Guid id)
        {
            var addresses = await _unitOfWork.AddressRepository.GetByIDAsync(id);
            if (addresses == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ.");
            }
            var respone = _mapper.Map<AddressDTO>(addresses);
            return new ApiResult<AddressDTO>(respone);
        }

        public async Task<ApiResult<AddressDTO>> CreateAddressAsync(AddressCreateDTO dto)
        {
            var Address = _mapper.Map<Address>(dto);

            await _unitOfWork.AddressRepository.AddAsync(Address);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<AddressDTO>(Address);
            return new ApiResult<AddressDTO>(response);
        }

        public async Task<ApiResult<AddressDTO>> UpdateAddressAsync(Guid id, AddressUpdateDTO dto)
        {
            var Address = await _unitOfWork.AddressRepository.GetByIDAsync(id);
            if (Address == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            _mapper.Map(dto, Address);
            _unitOfWork.AddressRepository.Update(Address);
            await _unitOfWork.SaveAsync();

            var respone = _mapper.Map<AddressDTO>(Address);
            return new ApiResult<AddressDTO>(respone, "Cập nhật địa chỉ thành công");
        }

        public async Task<ApiResult<bool>> DeleteAddressAsync(Guid id)
        {
            var Address = await _unitOfWork.AddressRepository.GetByIDAsync(id);

            if (Address == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            _unitOfWork.AddressRepository.Delete(Address);
            await _unitOfWork.SaveAsync();
            return new ApiResult<bool>(true, "Xóa địa chỉ thành công");
        }
    }
}

