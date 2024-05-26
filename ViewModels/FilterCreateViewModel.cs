using System.ComponentModel.DataAnnotations;

namespace Examensarbete.ViewModels
{
    public class FilterCreateViewModel
    {
        [Required]
        public string Text { get; set; }
        public List<CategoryCheckboxViewModel> Categories { get; set; }
        public FilterCreateViewModel()
        {
            Categories = new List<CategoryCheckboxViewModel>();
        }

    }
}
