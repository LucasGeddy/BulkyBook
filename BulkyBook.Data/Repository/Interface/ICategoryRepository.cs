using BulkyBook.Models;

namespace BulkyBook.Data.Repository.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
