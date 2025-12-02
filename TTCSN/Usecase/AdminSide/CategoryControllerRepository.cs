using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public class CategoryControllerRepository
    {
        private readonly ICategoryController repo;
        public CategoryControllerRepository(ICategoryController repository)
        {
            repo = repository;
        }
        public Task<bool> AddCategoryAsync(string categoryName)
        {
            return repo.AddCategoryAsync(categoryName);
        }
        public Task<bool> UpdateCategoryAsync(int categoryId, string categoryName)
        {
            return repo.UpdateCategoryAsync(categoryId, categoryName);
        }
        public Task<bool> DeleteCategoryAsync(int categoryId)
        {
            return repo.DeleteCategoryAsync(categoryId);
        }
        public Task<IEnumerable<Category>> GetCategories()
        {
            return repo.GetCategories();
        }
        public Task<Category?> GetCategory(int id)
        {
            return repo.GetCategory(id);
        }
    }
}
