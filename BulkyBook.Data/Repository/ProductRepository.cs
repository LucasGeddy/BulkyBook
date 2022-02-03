using BulkyBook.Data.Repository.Interface;
using BulkyBook.Models;

namespace BulkyBook.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var existingProduct = _db.Products.FirstOrDefault(r => r.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.CoverTypeId = product.CoverTypeId;
                existingProduct.ISBN = product.ISBN;
                existingProduct.Price = product.Price;
                existingProduct.Price50 = product.Price50;
                existingProduct.Price100 = product.Price100;
                existingProduct.ListPrice = product.ListPrice;
                existingProduct.Author = product.Author;
                existingProduct.ImageUrl = product.ImageUrl != null ? product.ImageUrl : existingProduct.ImageUrl;
            }
        }
    }
}
