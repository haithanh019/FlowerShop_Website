using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IFlowerService
    {
        IQueryable<FlowerDTO> GetFlowersOData();
        Task<ApiResponse<FlowerDTO>> GetFlowerByIDAsync(Guid id);
        Task<ApiResponse<FlowerDTO>> CreateFlowerAsync(FlowerCreateDTO dto);
        Task<ApiResponse<FlowerDTO>> UpdateFlowerAsync(Guid id, FlowerUpdateDTO dto);
        Task<ApiResponse<bool>> DeleteFlowerAsync(Guid id);
    }
}
