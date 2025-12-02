namespace TTCSN.Models
{
    public class CategoryListViewModel
    {
        public IEnumerable<CategoryViewModel> Categories { get; set; } = Enumerable.Empty<CategoryViewModel>();
    }
}
