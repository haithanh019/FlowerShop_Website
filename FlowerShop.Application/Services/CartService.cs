using AutoMapper;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResponse<CartDTO>> GetCartByUserIDAsync(Guid id)
        {
            var cart = await _unitOfWork.CartRepository.GetByAsync(
                c => c.UserID == id,
                includeProperties: "CartItems.Flower"
            );
            if (cart == null)
            {
                return new ApiResponse<CartDTO>("Không tìm thấy giỏ hàng của người dùng này.");
            }

            var cartDto = _mapper.Map<CartDTO>(cart);

            return new ApiResponse<CartDTO>
            {
                Success = true,
                Data = cartDto
            };
        }
    }
}
