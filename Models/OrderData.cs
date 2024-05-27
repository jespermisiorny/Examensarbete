using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Examensarbete.Models
{
    public class OrderData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // import-datum
        public bool IsConfirmed { get; set; } = false; // uppdateras efter validering
        public DateTime OrderDate { get; set; } // format: 2022-01-03
        public string? OrderGroup { get; set; } // Beställningsgrupp
        public string? SubArea { get; set; } // Delområde
        public string? ItemDescription { get; set; } // Artikelbeskrivning
        public string? ArticleNumber { get; set; } // Artikelnummer
        public string? SupplierName { get; set; } // Leverantörsnamn
        public string? UnitType { get; set; } // Enhetstyp
        public int? ConfirmedQuantity { get; set; } // Kvitterad
        public decimal? Price { get; set; } // Pris
        public decimal? ConfirmedNetAmount { get; set; } // Kvitterat nettobelopp
        public string? Account { get; set; } // Konto
        public string? CostCenter { get; set; } // Kostnadsställe
        public string? Organization { get; set; } // Organisation

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

    }
}
