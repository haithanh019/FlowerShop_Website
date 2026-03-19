using AutoMapper;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartItemDTO>> AddToCartAsync(CartItemCreateDTO dto)
        {
            var cart = await _unitOfWork.CartRepository.GetByIDAsync(dto.CartID) ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");

            var flower = await _unitOfWork.FlowerRepository.GetByAsync(
                f => f.FlowerID == dto.FlowerID,
                trackChanges: false,
                includeProperties: "FlowerImages"
            ) ?? throw new NotFoundException("Không tìm thấy hoa.");
            if (!flower.IsActive)
                throw new BadRequestException($"Hoa '{flower.FlowerName}' hiện không còn kinh doanh.");

            var existingItem = await _unitOfWork.CartItemRepository.GetByAsync(
                ci => ci.CartID == cart.CartID && ci.FlowerID == dto.FlowerID,
                trackChanges: true
            );

            CartItem cartItem;
            if (existingItem != null)
            {
                var newQty = existingItem.Quantity + dto.Quantity;

                if (flower.StockQuantity < newQty)
                    throw new BadRequestException(
                        $"Hoa '{flower.FlowerName}' chỉ còn {flower.StockQuantity} sản phẩm trong kho.");

                existingItem.Quantity = newQty;
                _unitOfWork.CartItemRepository.Update(existingItem);
                cartItem = existingItem;
            }
            else
            {
                if (flower.StockQuantity < dto.Quantity)
                    throw new BadRequestException(
                        $"Hoa '{flower.FlowerName}' chỉ còn {flower.StockQuantity} sản phẩm trong kho.");

                cartItem = _mapper.Map<CartItem>(dto);
                cartItem.CartID = cart.CartID;
                cartItem.UnitPrice = flower.Price;
                await _unitOfWork.CartItemRepository.AddAsync(cartItem);
            }

            await _unitOfWork.SaveAsync();

            cartItem.Flower = flower;
            var response = _mapper.Map<CartItemDTO>(cartItem);
            return new ApiResponse<CartItemDTO>(response, "Thêm vào giỏ hàng thành công");
        }

        public async Task<ApiResponse<CartItemDTO>> UpdateCartItemAsync(Guid id, CartItemUpdateDTO dto)
        {
            var cartItem = await _unitOfWork.CartItemRepository.GetByAsync(
                ci => ci.CartItemID == id,
                trackChanges: true,
                includeProperties: "Flower,Flower.FlowerImages"
            );
            if (cartItem == null)
                throw new NotFoundException("Không tìm thấy sản phẩm trong giỏ hàng.");

            if (cartItem.Flower!.StockQuantity < dto.Quantity)
                throw new BadRequestException(
                    $"Hoa '{cartItem.Flower.FlowerName}' chỉ còn {cartItem.Flower.StockQuantity} sản phẩm trong kho.");

            _mapper.Map(dto, cartItem);
            _unitOfWork.CartItemRepository.Update(cartItem);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<CartItemDTO>(cartItem);
            return new ApiResponse<CartItemDTO>(response, "Cập nhật giỏ hàng thành công");
        }

        public async Task<ApiResponse<bool>> RemoveCartItemAsync(Guid id)
        {
            var cartItem = await _unitOfWork.CartItemRepository.GetByIDAsync(id) ?? throw new NotFoundException("Không tìm thấy sản phẩm trong giỏ hàng.");
            _unitOfWork.CartItemRepository.Delete(cartItem);
            await _unitOfWork.SaveAsync();
            return new ApiResponse<bool>(true, "Xóa sản phẩm khỏi giỏ hàng thành công");
        }

        public Task<ApiResponse<CartItemDTO>> AddToCartAsync(Guid id, CartItemCreateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
