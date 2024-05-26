using System.ComponentModel.DataAnnotations;

namespace Examensarbete.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Förälder")]
        public int? ParentId { get; set; }

        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public List<ProductCategory> ProductCategories { get; set; }

        public List<FilterCategory> FilterCategories { get; set; }

    }
}
