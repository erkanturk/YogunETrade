using ETICARET.Entities;

namespace ETICARET.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<Image> Images { get; set; }
        public List<Category> SelectedCategories { get; set; }
        public string CategoryId { get; set; }
        public ProductModel()
        {
            Images = new List<Image>();
        }

    }
}
