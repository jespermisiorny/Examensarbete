using System.ComponentModel.DataAnnotations;

namespace Examensarbete.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Inaktiv")]
        public bool IsInactive { get; set; } = false;

        [Display(Name = "Ej Komplett")]
        public bool IsIncomplete { get; set; }

        [Display(Name = "Namn")]
        public string? Name { get; set; }

        [Display(Name = "Artikelnummer")]
        public string? ArticleNumber { get; set; }

        [Display(Name = "Pris per enhet")]
        public decimal PricePerUnit { get; set; }

        [Display(Name = "Vikt per enhet")]
        public double WeightPerUnit { get; set; }

        [Display(Name = "Återvunningsgrad vid EoL")]
        public double RecyclingRateAtEoL { get; set; }

        public int? PackagingMaterialId { get; set; }
        [Display(Name = "Förpackningsmaterial")]
        public virtual Material? PackagingMaterial { get; set; }


        public List<ProductMaterial>? ProductMaterials { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
        public virtual ICollection<OrderData>? OrderDatas { get; set; } = new List<OrderData>();
        public double TotalClimateImpactPerUnit => CalculateTotalClimateImpactPerUnit();
        public double TotalClimateImpactCradleToGate => CalculateTotalClimateImpactCradleToGate();
        public double TotalClimateImpactImpactEoL => CalculateTotalClimateImpactEoL();
        public double TotalClimateImpactPackagingMaterial => CalculateTotalClimateImpactPackagingMaterial();


        public Product()
        {
            ProductMaterials = new List<ProductMaterial>();
            ProductCategories = new List<ProductCategory>();
        }


        private double CalculateTotalClimateImpactPerUnit()
        {
            return 0;
        }


        private double CalculateTotalClimateImpactCradleToGate()
        {
            return 0;
        }


        private double CalculateTotalClimateImpactEoL()
        {
            return 0;
        }


        private double CalculateTotalClimateImpactPackagingMaterial()
        {
            return 0;
        }
    }
}
