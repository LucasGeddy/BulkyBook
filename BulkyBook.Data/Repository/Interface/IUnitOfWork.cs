namespace BulkyBook.Data.Repository.Interface
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        Task Save();
    }
}
