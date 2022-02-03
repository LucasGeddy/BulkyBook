using BulkyBook.Models;

namespace BulkyBook.Data.Repository.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
