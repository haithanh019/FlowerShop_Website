using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IFlowerService
    {
        IQueryable<FlowerDTO> GetFlowersOData();
        Task<ApiResult<FlowerDTO>> GetFlowerByIDAsync(Guid id);
        Task<ApiResult<FlowerDTO>> CreateFlowerAsync(FlowerCreateDTO dto);
        Task<ApiResult<FlowerDTO>> UpdateFlowerAsync(Guid id, FlowerUpdateDTO dto);
        Task<ApiResult<bool>> DeleteFlowerAsync(Guid id);
    }
}
