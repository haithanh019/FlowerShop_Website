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
                cart = new Cart
                {
                    CartID = Guid.NewGuid(),
                    UserID = id,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();
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
