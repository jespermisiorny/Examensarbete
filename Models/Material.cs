using System.ComponentModel.DataAnnotations;

namespace Examensarbete.Models
{
    public class Material
    {
        public int Id { get; set; }

        [Display(Name = "Typ av material")]
        public string? Type { get; set; }

        [Display(Name = "Förvald Produktionsprocess")]
        public string? DefaultProductionProcess { get; set; }

        [Display(Name = "UF material (nytt)")]
        public decimal EFMaterialNew { get; set; }

        [Display(Name = "UF produktionsprocess")]
        public decimal EFProductionProcess { get; set; }

        [Display(Name = "UF EoL Förbränning")]
        public decimal EFEoLIncineration { get; set; }

        [Display(Name = "UF EoL återvinning")]
        public decimal EFEoLRecycling { get; set; }

        [Display(Name = "Återvunnet material i material")]
        public decimal RecycledContentInMaterial { get; set; }

        [Display(Name = "UF material (återvunnet)")]
        public decimal EFMaterialRecycled { get; set; }

        [Display(Name = "Återvunningsgrad i EoL")]
        public decimal RecyclingRateAtEoL { get; set; }

        public List<ProductMaterial>? ProductMaterials { get; set; }

    }
}
