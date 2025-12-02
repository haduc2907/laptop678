using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public interface ICategoryController
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<bool> AddCategoryAsync(string categoryName);
        Task<bool> UpdateCategoryAsync(int categoryId, string categoryName);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
