namespace Examensarbete.ViewModels
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string ArticleNumber { get; set; }
        public decimal PricePerUnit { get; set; }
        public double WeightPerUnit { get; set; }
        public double RecyclingRateAtEoL { get; set; }
        public List<MaterialViewModel> Materials { get; set; } = new List<MaterialViewModel>();
        public List<CategoryCheckboxViewModel> Categories { get; set; } = new List<CategoryCheckboxViewModel>();

        public ProductViewModel()
        {
            Materials = new List<MaterialViewModel>();
            Categories = new List<CategoryCheckboxViewModel>();
        }
    }
}
