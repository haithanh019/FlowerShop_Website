using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
