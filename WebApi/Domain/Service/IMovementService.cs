using System.Threading.Tasks;
using WebApi.Domain.enums;
using WebApi.Domain.Model;
using WebApi.Infrastructure.Dto.Request;

namespace WebApi.Domain.Service
{
    public interface IMovementService
    {
        Task<Product> SaleOrBuyProducts(SaleBuy saleBuy, MovementType type);

        Task TranslateProductStore(TranslateRequest translate);
    }
}
