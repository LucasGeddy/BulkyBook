using BulkyBook.Models;

namespace BulkyBook.Data.Repository.Interface
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
